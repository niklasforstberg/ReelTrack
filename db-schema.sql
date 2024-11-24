-- Users/Family Members
CREATE TABLE Users (
    UserId INTEGER PRIMARY KEY,
    Email TEXT UNIQUE NOT NULL,
    PasswordHash TEXT NOT NULL,
    Name TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Families
CREATE TABLE Families (
    FamilyId INTEGER PRIMARY KEY,
    Name TEXT NOT NULL,
    InviteCode TEXT UNIQUE NOT NULL,
    AdminUserId INTEGER NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AdminUserId) REFERENCES Users(UserId)
);

-- Family Members (junction table)
CREATE TABLE FamilyMembers (
    FamilyId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    JoinedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (FamilyId, UserId),
    FOREIGN KEY (FamilyId) REFERENCES Families(FamilyId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Watch Lists
CREATE TABLE WatchLists (
    ListId INTEGER PRIMARY KEY,
    FamilyId INTEGER NOT NULL,
    Name TEXT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (FamilyId) REFERENCES Families(FamilyId) ON DELETE CASCADE
);

-- Watch List Members (who's part of which list)
CREATE TABLE WatchListMembers (
    ListId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    PRIMARY KEY (ListId, UserId),
    FOREIGN KEY (ListId) REFERENCES WatchLists(ListId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Watch List Member Order (for tracking turns)
CREATE TABLE WatchListMemberOrder (
    ListId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    TurnOrder INTEGER NOT NULL,
    PRIMARY KEY (ListId, UserId),
    FOREIGN KEY (ListId) REFERENCES WatchLists(ListId) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    UNIQUE(ListId, TurnOrder)
);

-- Movies
CREATE TABLE Movies (
    MovieId INTEGER PRIMARY KEY,
    ImdbId TEXT UNIQUE NOT NULL,
    Title TEXT NOT NULL
);

-- Watch History
CREATE TABLE WatchHistory (
    WatchId INTEGER PRIMARY KEY,
    ListId INTEGER NOT NULL,
    MovieId INTEGER NOT NULL,
    ChoosenByUserId INTEGER NOT NULL,
    WatchedAt DATETIME NOT NULL,
    FOREIGN KEY (ListId) REFERENCES WatchLists(ListId) ON DELETE CASCADE,
    FOREIGN KEY (MovieId) REFERENCES Movies(MovieId),
    FOREIGN KEY (ChoosenByUserId) REFERENCES Users(UserId)
);