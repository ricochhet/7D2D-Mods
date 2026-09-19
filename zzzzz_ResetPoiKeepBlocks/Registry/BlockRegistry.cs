using System;
using System.Collections.Generic;
using System.IO;

namespace ResetPoiKeepBlocks.Registry;

public static class BlockRegistry
{
    private const string FileName = "ResetPoiKeepBlocks.dat";
    private const int FileVersion = 1;

    private static readonly HashSet<Vector3i> Positions = [];
    private static bool _dirty;
    private static bool _loaded;

    public static void MarkPlaced(Vector3i pos)
    {
        if (Positions.Add(pos))
        {
            _dirty = true;
            SaveIfDirty();
        }
    }

    public static void MarkRemoved(Vector3i pos)
    {
        if (Positions.Remove(pos))
        {
            _dirty = true;
            SaveIfDirty();
        }
    }

    public static bool IsPlayerPlaced(Vector3i pos) => Positions.Contains(pos);

    public static void Load()
    {
        Positions.Clear();
        _dirty = false;
        _loaded = true;

        string path = GetFilePath();
        if (!File.Exists(path))
            return;

        try
        {
            using FileStream stream = File.OpenRead(path);
            using BinaryReader reader = new(stream);
            int version = reader.ReadInt32();
            if (version != FileVersion)
            {
                Log.Warning(ModSettings.LogPrefix + $"Unexpected registry version: {version}");
                return;
            }

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int z = reader.ReadInt32();
                Positions.Add(new Vector3i(x, y, z));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ModSettings.LogPrefix + "loading registry failed: " + ex.GetBaseException().Message);
            Log.Exception(ex);
        }
    }

    public static void SaveIfDirty()
    {
        if (!_loaded || !_dirty)
            return;

        try
        {
            string path = GetFilePath();
            using (FileStream stream = File.Create(path))
            using (BinaryWriter writer = new(stream))
            {
                writer.Write(FileVersion);
                writer.Write(Positions.Count);
                foreach (Vector3i pos in Positions)
                {
                    writer.Write(pos.x);
                    writer.Write(pos.y);
                    writer.Write(pos.z);
                }
            }

            _dirty = false;
        }
        catch (Exception ex)
        {
            Log.Error(ModSettings.LogPrefix + "saving registry failed: " + ex.GetBaseException().Message);
            Log.Exception(ex);
        }
    }

    public static void Reset()
    {
        Positions.Clear();
        _dirty = false;
        _loaded = false;
    }

    private static string GetFilePath() => Path.Combine(GameIO.GetSaveGameDir(), FileName);
}
