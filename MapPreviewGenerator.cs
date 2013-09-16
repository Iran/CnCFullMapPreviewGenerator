﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Nyerguds.Ini;
using System.Drawing;

namespace CncFullMapPreviewGenerator
{
    class MapPreviewGenerator
    {
        const int CellSize = 24; // in pixels
        static bool IsLoaded = false;
        IniFile MapINI;
        IniFile TemplatesINI;
        static IniFile TilesetsINI;
        CellStruct[,] Cells = new CellStruct[64, 64];
        WaypointStruct[] Waypoints = new WaypointStruct[8];
        static Bitmap[] SpawnLocationBitmaps = new Bitmap[8];
        int MapWidth = -1, MapHeight = -1, MapY = -1, MapX = -1;

        public static void Load()
        {
            TilesetsINI = new IniFile("data/tilesets.ini");
        }

        public MapPreviewGenerator(string FileName)
        {

            MapINI = new IniFile(FileName);

            MapHeight = MapINI.getIntValue("Map", "Height", -1);
            MapWidth = MapINI.getIntValue("Map", "Width", -1);
            MapX = MapINI.getIntValue("Map", "X", -1);
            MapY = MapINI.getIntValue("Map", "Y", -1);

            string MapBin = FileName.Replace(".ini", ".bin");

            Console.WriteLine("MapBin = {0}, FileName = {1}", MapBin, FileName);

            CellStruct[] Raw = new CellStruct[64*64];

            byte[] fileBytes = File.ReadAllBytes(MapBin);

            var ByteReader = new FastByteReader(fileBytes);

            int i = 0;
            while (!ByteReader.Done())
            {

                Raw[i].Template = ByteReader.ReadByte();
                Raw[i].Tile = ByteReader.ReadByte();

                //                Console.WriteLine("{0} = {1}", i, Cells[i].Template);
                ++i;

                if (i == 64 * 64)
                    break;

            }

            var SectionKeyValues = MapINI.getSectionContent("Waypoints");

            foreach (KeyValuePair<string, string> entry in SectionKeyValues)
            {
                int WayPoint = int.Parse(entry.Key);
                int Cell = int.Parse(entry.Value);

                Raw[Cell].Waypoint = WayPoint + 1;

                //                Console.WriteLine("{0} = {1}", WayPoint, Cell);
            }

            var SectionTerrrain = MapINI.getSectionContent("Terrain");

            if (SectionTerrrain != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionTerrrain)
                {
                    int Cell = int.Parse(entry.Key);
                    string Terrain = entry.Value;

                    Raw[Cell].Terrain = Terrain;

                    //                Console.WriteLine("{0} = {1}", Cell, Terrain);
                }
            }
            var SectionOverlay = MapINI.getSectionContent("Overlay");

            if (SectionOverlay != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionOverlay)
                {
                    int Cell = int.Parse(entry.Key);
                    string Overlay = entry.Value;

                    Raw[Cell].Overlay = Overlay;

                    //                Console.WriteLine("{0} = {1}", Cell, Terrain);
                }
            }

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    int Index = (x * 64) + y;
                    Cells[y, x] = Raw[Index];

                    int WayPoint = Raw[Index].Waypoint - 1;
                    if (WayPoint >= 0 && WayPoint < 8)
                    {
                        //                        Console.WriteLine("Waypoint found! ID = {0}, Raw = {1}", WayPoint, Index);

                        Waypoints[WayPoint].WasFound = true;
                        Waypoints[WayPoint].X = x;
                        Waypoints[WayPoint].Y = y;
                    }
                }
            }
        }


        public Bitmap Get_Bitmap()
        {
            Bitmap bitMap = new Bitmap(64 * CellSize, 64 * CellSize);
            Graphics g = Graphics.FromImage(bitMap);


            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    CellStruct data = Cells[x, y];

                    Draw_Template(data, g, x, y);

                    if (data.Terrain != null)
                    {
                        // draw terrain shit
                    }

                    else if (data.Overlay != null)
                    {
                        // draw overlay shit
                    }

                    if (Is_Out_Of_Bounds(x, y))
                    {
                        // whatever
                    }

                }
            }

//            Graphics g = Graphics.FromImage(_Bitmap);

//            Draw_Spawn_Locations(ref g, _ScaleFactor);
//            g.Flush();

            return bitMap;
        }

        void Draw_Template(CellStruct Cell, Graphics g, int X, int Y)
        {
//            if (Cell.Tile != 0 ) { return; }

            int[] ShadowIndex = { };
            Palette Pal = Palette.Load("data/temperat.pal", ShadowIndex);

            string TemplateString = TilesetsINI.getStringValue("TileSets", Cell.Template.ToString(), "0");

            TemplateReader Temp = TemplateReader.Load("data/" + TemplateString + ".tem");

            Bitmap TempBitmap = RenderUtils.RenderTemplate(Temp, Pal, Cell.Tile);
            g.DrawImage(TempBitmap, X * CellSize, Y * CellSize, TempBitmap.Width, TempBitmap.Height);
        }

        bool Is_Out_Of_Bounds(int X, int Y)
        {
            if (MapX > X || X >= MapX + MapWidth)
                return true;

            if (MapY > Y || Y >= MapY + MapHeight)
                return true;

            return false;
        }

    }

    struct CellStruct
    {
        public int Template;
        public int Tile;
        public string Overlay;
        public string Terrain;
        public int Waypoint;
    }

    struct WaypointStruct
    {
        public bool WasFound;
        public int X;
        public int Y;
    }

    enum TerrainType
    {
        Clear = 0,
        Water,
        Road,
        Rock,
        Tree,
        River,
        Rough,
        Wall,
        Beach,
        Ore,
        Gems,
    }
}
