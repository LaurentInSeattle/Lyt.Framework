﻿namespace Lyt.Persistence;

using static FileManagerModel;

public sealed record class FileId(Area Area, Kind Kind, string Filename);