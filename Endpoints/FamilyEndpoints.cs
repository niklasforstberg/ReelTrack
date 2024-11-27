using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ReelTrack.Models;
using ReelTrack.Models.DTOs;
using ReelTrack.Helpers;

namespace ReelTrack.Endpoints;

public static class FamilyEndpoints
{
    public static void MapFamilyEndpoints(this WebApplication app)
    {
        app.MapGet("/family", [Authorize] async (HttpContext ctx, ReelTrackDbContext db) =>
        {
            var familyId = ctx.User.FindFirst("familyId")?.Value;
            if (string.IsNullOrEmpty(familyId)) return Results.NotFound();

            var family = await db.Families
                .Include(f => f.Members)
                .FirstOrDefaultAsync(f => f.Id == int.Parse(familyId));

            if (family == null) return Results.NotFound();

            var familyDto = new FamilyDto
            {
                Id = family.Id,
                Name = family.Name,
                InviteCode = family.InviteCode,
                Members = family.Members.Select(m => new UserDto(
                    m.Id,
                    m.Email,
                    m.Name,
                    m.IsAdmin,
                    m.FamilyId
                )).ToList()
            };

            return Results.Ok(familyDto);
        })
        .WithName("GetFamily")
        .WithOpenApi();

        app.MapGet("/family/members", [Authorize] async (HttpContext ctx, ReelTrackDbContext db) =>
        {
            var familyId = ctx.User.FindFirst("familyId")?.Value;
            if (string.IsNullOrEmpty(familyId)) return Results.NotFound();

            var members = await db.Users
                .Where(u => u.FamilyId == int.Parse(familyId))
                .Select(u => new { u.Id, u.Name, u.Email, u.IsAdmin })
                .ToListAsync();

            return Results.Ok(members);
        })
        .WithName("GetFamilyMembers")
        .WithOpenApi();

        app.MapPost("/family", [Authorize] async (CreateFamilyRequest request, HttpContext ctx, ReelTrackDbContext db) =>
        {
            var userId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await db.Users.FindAsync(userId);
            if (user == null) return Results.NotFound();
            if (user.FamilyId != null) return Results.BadRequest("User already belongs to a family");

            string inviteCode;
            do
            {
                inviteCode = new string(Enumerable.Range(0, 5)
                    .Select(_ => "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"[Random.Shared.Next(32)])
                    .ToArray());
            } while (await db.Families.AnyAsync(f => f.InviteCode == inviteCode));

            var family = new Family
            {
                Name = request.Name,
                InviteCode = inviteCode
            };

            db.Families.Add(family);
            await db.SaveChangesAsync();

            user.FamilyId = family.Id;
            user.IsAdmin = true;
            await db.SaveChangesAsync();

            return Results.Ok(new FamilyDto
            {
                Id = family.Id,
                Name = family.Name,
                InviteCode = family.InviteCode,
                Members = new List<UserDto>
                {
                    new UserDto(user.Id, user.Email, user.Name, user.IsAdmin, user.FamilyId)
                }
            });
        })
        .WithName("CreateFamily")
        .WithOpenApi();

        app.MapPut("/family", [Authorize] async (UpdateFamilyRequest request, HttpContext ctx, ReelTrackDbContext db) =>
        {
            var familyId = ctx.User.FindFirst("familyId")?.Value;
            var isAdmin = ctx.User.IsInRole("Admin");

            if (string.IsNullOrEmpty(familyId) || !isAdmin)
                return Results.Forbid();

            var family = await db.Families.FindAsync(int.Parse(familyId));
            if (family == null) return Results.NotFound();

            family.Name = request.Name;
            await db.SaveChangesAsync();

            return Results.Ok(family);
        })
        .WithName("UpdateFamily")
        .WithOpenApi();

        app.MapDelete("/family", [Authorize] async (HttpContext ctx, ReelTrackDbContext db) =>
        {
            var familyId = ctx.User.FindFirst("familyId")?.Value;
            var isAdmin = ctx.User.IsInRole("Admin");

            if (string.IsNullOrEmpty(familyId) || !isAdmin)
                return Results.Forbid();

            var family = await db.Families.FindAsync(int.Parse(familyId));
            if (family == null) return Results.NotFound();

            db.Families.Remove(family);
            await db.SaveChangesAsync();

            return Results.Ok();
        })
        .WithName("DeleteFamily")
        .WithOpenApi();
    }
}