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
        static Dictionary<string, int> TiberiumStages = new Dictionary<string, int>();
        static Random MapRandom;
        const int CellSize = 24; // in pixels
        static bool IsLoaded = false;
        string Theater;
        IniFile MapINI;
        IniFile TemplatesINI;
        static IniFile TilesetsINI;
        string TheaterFilesExtension;
        Palette Pal;
        CellStruct[,] Cells = new CellStruct[64, 64];
        List<WaypointStruct> Waypoints = new List<WaypointStruct>();
        List<UnitInfo> Units = new List<UnitInfo>();
        List<InfantryInfo> Infantries = new List<InfantryInfo>();
        List<StructureInfo> Structures = new List<StructureInfo>();
        List<BibInfo> Bibs = new List<BibInfo>();
        static Dictionary<string, BuildingBibInfo> BuildingBibs = new Dictionary<string, BuildingBibInfo>();
        static Dictionary<string, int> BuildingDamageFrames = new Dictionary<string, int>();

        static Bitmap[] SpawnLocationBitmaps = new Bitmap[8];
        int MapWidth = -1, MapHeight = -1, MapY = -1, MapX = -1;

        public static void Load()
        {
            TilesetsINI = new IniFile("data/tilesets.ini");
            MapRandom = new Random();;

            Load_Building_Damage_Frames();
            Load_Building_Bibs();

            TiberiumStages.Add("ti1", 0);
            TiberiumStages.Add("ti2", 1);
            TiberiumStages.Add("ti3", 2);
            TiberiumStages.Add("ti4", 3);
            TiberiumStages.Add("ti5", 4);
            TiberiumStages.Add("ti6", 5);
            TiberiumStages.Add("ti7", 6);
            TiberiumStages.Add("ti8", 7);
            TiberiumStages.Add("ti9", 8);
            TiberiumStages.Add("ti10", 9);
            TiberiumStages.Add("ti11", 10);
            TiberiumStages.Add("ti12", 11);
        }

        static void Load_Building_Bibs()
        {
            BuildingBibs.Add("afld", new BuildingBibInfo("bib1", 1));

            BuildingBibs.Add("bio", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("eye", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("fact", new BuildingBibInfo("bib2", 1));
            BuildingBibs.Add("fix", new BuildingBibInfo("bib2", 2));
            BuildingBibs.Add("hand", new BuildingBibInfo("bib3", 2));
            BuildingBibs.Add("hosp", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("hpad", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("hq", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("miss", new BuildingBibInfo("bib2", 1));
            BuildingBibs.Add("nuke", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("nuk2", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("proc", new BuildingBibInfo("bib2", 2));
            BuildingBibs.Add("pyle", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("silo", new BuildingBibInfo("bib3", 0));
            BuildingBibs.Add("weap", new BuildingBibInfo("bib2", 2));
            BuildingBibs.Add("tmpl", new BuildingBibInfo("bib2", 2));

        }
        static void Load_Building_Damage_Frames()
        {
            // the GUN turret building requires special logic as it 
            // might make use of the angle property

            BuildingDamageFrames.Add("afld", 16);
            BuildingDamageFrames.Add("arco", 1);
            BuildingDamageFrames.Add("atwr", 1);
            BuildingDamageFrames.Add("bio", 1);
            BuildingDamageFrames.Add("eye", 16);
            BuildingDamageFrames.Add("fact", 24);
            BuildingDamageFrames.Add("fix", 7);
            BuildingDamageFrames.Add("gtwr", 1);
            BuildingDamageFrames.Add("gun", 64);
            BuildingDamageFrames.Add("hand", 1);
            BuildingDamageFrames.Add("hosp", 4);
            BuildingDamageFrames.Add("hpad", 7);
            BuildingDamageFrames.Add("hq", 16);
            BuildingDamageFrames.Add("miss", 1);
            BuildingDamageFrames.Add("nuk2", 4);
            BuildingDamageFrames.Add("nuke", 4);
            BuildingDamageFrames.Add("obli", 4);
            BuildingDamageFrames.Add("proc", 30);
            BuildingDamageFrames.Add("pyle", 10);
            BuildingDamageFrames.Add("sam", 64);
            BuildingDamageFrames.Add("silo", 5);
            BuildingDamageFrames.Add("weap2", 10);
            BuildingDamageFrames.Add("v01", 1);
            BuildingDamageFrames.Add("v02", 1);
            BuildingDamageFrames.Add("v03", 1);
            BuildingDamageFrames.Add("v04", 1);
            BuildingDamageFrames.Add("v05", 1);
            BuildingDamageFrames.Add("v06", 1);
            BuildingDamageFrames.Add("v07", 1);
            BuildingDamageFrames.Add("v08", 1);
            BuildingDamageFrames.Add("v09", 1);
            BuildingDamageFrames.Add("v10", 1);
            BuildingDamageFrames.Add("v11", 1);
            BuildingDamageFrames.Add("v12", 1);
            BuildingDamageFrames.Add("v13", 1);
            BuildingDamageFrames.Add("v14", 1);
            BuildingDamageFrames.Add("v15", 1);
            BuildingDamageFrames.Add("v16", 1);
            BuildingDamageFrames.Add("v17", 1);
            BuildingDamageFrames.Add("v18", 1);
            BuildingDamageFrames.Add("v19", 14);
            BuildingDamageFrames.Add("v20", 3);
            BuildingDamageFrames.Add("v21", 3);
            BuildingDamageFrames.Add("v22", 3);
            BuildingDamageFrames.Add("v23", 3);
            BuildingDamageFrames.Add("v24", 1);
            BuildingDamageFrames.Add("v25", 1);
            BuildingDamageFrames.Add("v26", 1);
            BuildingDamageFrames.Add("v27", 1);
            BuildingDamageFrames.Add("v28", 1);
            BuildingDamageFrames.Add("v29", 1);
            BuildingDamageFrames.Add("v30", 1);
            BuildingDamageFrames.Add("v31", 1);
            BuildingDamageFrames.Add("v32", 1);
            BuildingDamageFrames.Add("v33", 1);
            BuildingDamageFrames.Add("v34", 1);
            BuildingDamageFrames.Add("v35", 1);
            BuildingDamageFrames.Add("v36", 1);
            BuildingDamageFrames.Add("v37", 1);
            BuildingDamageFrames.Add("tmpl", 5);
        }

        int Frame_From_Building_HP(StructureInfo s)
        {
            if (s.HP > 128) { return 0; }

            int Frame;
            BuildingDamageFrames.TryGetValue(s.Name, out Frame);

            return Frame;
        }

        public MapPreviewGenerator(string FileName)
        {

            MapINI = new IniFile(FileName);

            MapHeight = MapINI.getIntValue("Map", "Height", -1);
            MapWidth = MapINI.getIntValue("Map", "Width", -1);
            MapX = MapINI.getIntValue("Map", "X", -1);
            MapY = MapINI.getIntValue("Map", "Y", -1);

            Parse_Theater();

            string MapBin = FileName.Replace(".ini", ".bin");

            Console.WriteLine("MapBin = {0}, FileName = {1}", MapBin, FileName);

            CellStruct[] Raw = new CellStruct[64*64];

            byte[] fileBytes = File.ReadAllBytes(MapBin);

            var ByteReader = new FastByteReader(fileBytes);


            // Parse templates
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

            Parse_Waypoints();
            Parse_Terrain(Raw);
            Parse_Overlay(Raw);
            Parse_Units();
            Parse_Infantry();
            Parse_Structures();

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    int Index = (x * 64) + y;
                    Cells[y, x] = Raw[Index];
                }
            }
        }

        void Parse_Waypoints()
        {
            var SectionKeyValues = MapINI.getSectionContent("Waypoints");

            foreach (KeyValuePair<string, string> entry in SectionKeyValues)
            {
                int WayPoint = int.Parse(entry.Key);
                int CellIndex = int.Parse(entry.Value);

                Console.WriteLine("Waypoint = {0}, Index = {1}", WayPoint, CellIndex);
                WaypointStruct WP = new WaypointStruct();
                WP.Number = WayPoint;
                WP.X =  CellIndex % 64;
                WP.Y = CellIndex / 64;
                Waypoints.Add(WP);
            }
        }

        void Parse_Overlay(CellStruct[] Raw)
        {
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
        }

        void Parse_Terrain(CellStruct[] Raw)
        {
            var SectionTerrrain = MapINI.getSectionContent("Terrain");

            if (SectionTerrrain != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionTerrrain)
                {
                    int Cell = int.Parse(entry.Key);
                    string Terrain = entry.Value;

                    Raw[Cell].Terrain = Terrain;

                    // Console.WriteLine("{0} = {1}", Cell, Terrain);
                }
            }
        }

        void Parse_Theater()
        {

            Theater = MapINI.getStringValue("Map", "Theater", "temperate");
            Theater = Theater.ToLower();

            switch (Theater)
            {
                case "winter": TheaterFilesExtension = ".win"; break;
                case "snow": TheaterFilesExtension = ".sno"; break;
                case "desert": TheaterFilesExtension = ".des"; break;
                default: TheaterFilesExtension = ".tem"; break;
            }

            string PalName = "temperat";

            switch (Theater)
            {
                case "winter": PalName = "winter"; break;
                case "snow": PalName = "snow"; break;
                case "desert": PalName = "desert"; break;
                default: PalName = "temperat";  break;
            }

            int[] ShadowIndex = { 3, 4 };
            Pal = Palette.Load("data/" + Theater + "/" + PalName + ".pal", ShadowIndex);
        }

        void Sub_Cell_Pixel_Offsets(int SubCell, out int X, out int Y)
        {
            X = -19; Y = -9;

            switch (SubCell)
            {
                case 1: X += 0; Y += 0; break;
                case 2: X += 11; Y += 0; break;
                case 3: Y += 11; break;
                case 4: X += 11; Y += 11; break;
                case 0: X += 6; Y += 6; break;
                default: break;
            }
        }

        void Parse_Units()
        {
            var SectionUnits = MapINI.getSectionContent("Units");
            if (SectionUnits != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionUnits)
                {
                    string UnitCommaString = entry.Value;
                    string[] UnitData = UnitCommaString.Split(',');

                    UnitInfo u = new UnitInfo();
                    u.Name = UnitData[1];
                    u.Side = UnitData[0];
                    u.Angle = int.Parse(UnitData[4]);

                    int CellIndex = int.Parse(UnitData[3]);
                    u.Y = CellIndex / 64;
                    u.X = CellIndex % 64;

                    Units.Add(u);

                    Console.WriteLine("Unit name = {0}, side {1}, Angle = {2}, X = {3}, Y = {4}", u.Name,
                        u.Side, u.Angle, u.X, u.Y);
                }
            }
        }

        void Parse_Structures()
        {
            var SectionStructures = MapINI.getSectionContent("Structures");
            if (SectionStructures != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionStructures)
                {
                    string StructCommaString = entry.Value;
                    string[] StructData = StructCommaString.Split(',');

                    // 0=neutral,afld,256,6,0,none
                    StructureInfo s = new StructureInfo();
                    s.Name = StructData[1];
                    s.Side = StructData[0];
                    s.Angle = int.Parse(StructData[4]);
                    s.HP = int.Parse(StructData[2]);
                    int CellIndex = int.Parse(StructData[3]);
                    s.Y = CellIndex / 64;
                    s.X = CellIndex % 64;

                    Structures.Add(s);

                    if (s.Name.ToLower() == "weap")
                    {
                        StructureInfo s2 = new StructureInfo();
                        s2.Name = "weap2";
                        s2.Side = s.Side;
                        s2.Angle = s.Angle;
                        s2.HP = s.HP;
                        s2.Y = s.Y;
                        s2.X = s.X;

                        Structures.Add(s2);
                    }

                    if (BuildingBibs.ContainsKey(s.Name))
                    {
                        BuildingBibInfo bi = new BuildingBibInfo();
                        BuildingBibs.TryGetValue(s.Name, out bi);

                        BibInfo bib = new BibInfo();
                        bib.Name = bi.Name; 
                        bib.X = s.X;
                        bib.Y = s.Y + bi.Yoffset;

                        Bibs.Add(bib);
                    }

                    Console.WriteLine("structure name = {0}, side {1}, HP = {5}, Angle = {2}, X = {3}, Y = {4}", s.Name,
                        s.Side, s.Angle, s.X, s.Y, s.HP);
                }
            }
        }


        void Parse_Infantry()
        {
            // 0=neutral,c1,256,2973,2,guard,3,none
            var SectionInfantry = MapINI.getSectionContent("Infantry");
            if (SectionInfantry != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionInfantry)
                {
                    string InfCommaString = entry.Value;
                    string[] InfData = InfCommaString.Split(',');

                    InfantryInfo inf = new InfantryInfo();
                    inf.Name = InfData[1];
                    inf.Side = InfData[0];
                    inf.Angle = int.Parse(InfData[6]);
                    inf.SubCell = int.Parse(InfData[4]);

                    int CellIndex = int.Parse(InfData[3]);
                    inf.Y = CellIndex / 64;
                    inf.X = CellIndex % 64;

                    Infantries.Add(inf);

                    int subX; int subY;
                    Sub_Cell_Pixel_Offsets(inf.SubCell, out subX, out subY);

                    Console.WriteLine("infantry name = {0}, Side = {1}, Angle = {2}, SubCell = {5}, X = {3}, Y = {4}", inf.Name,
                        inf.Side, inf.Angle, inf.X + subX, inf.Y + subY, inf.SubCell);
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
                }
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    CellStruct data = Cells[x, y];

                    if (data.Terrain != null)
                    {
                        Draw_Terrain(data, g, x, y);
                    }
                }
            }


            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    CellStruct data = Cells[x, y];

                    if (data.Overlay != null)
                    {
                         Draw_Overlay(data, g, x, y);
                    }
                }
            }

            Draw_Bibs(g);
            Draw_Structures(g);
            Draw_Units(g);
            Draw_Infantries(g);



/*            if (Is_Out_Of_Bounds(x, y))
            {
                // whatever
            }*/

//            Graphics g = Graphics.FromImage(_Bitmap);

//            Draw_Spawn_Locations(ref g, _ScaleFactor);
//            g.Flush();

            return bitMap;
        }

        void Draw_Units(Graphics g)
        {
            foreach (UnitInfo u in Units)
            {
                Draw_Unit(u, g);
            }

        }

        void Draw_Unit(UnitInfo u, Graphics g)
        {
            ShpReader UnitShp = ShpReader.Load(General_File_String_From_Name(u.Name));

            Bitmap UnitBitmap = RenderUtils.RenderShp(UnitShp, Pal, Frame_From_Unit_Angle(u.Angle));

            Draw_Centered(g, UnitBitmap, u);
        }

        void Draw_Centered(Graphics g, Bitmap bitMap, UnitInfo u)
        {
            int X = (u.X * CellSize) + 12 - (bitMap.Width / 2);
            int Y = (u.Y * CellSize) + 12 - (bitMap.Height / 2);

            g.DrawImage(bitMap, X, Y, bitMap.Width, bitMap.Height);
        }

        void Draw_Infantries(Graphics g)
        {
            foreach (InfantryInfo i in Infantries)
            {
                Draw_Infantry(i, g);
            }

        }

        void Draw_Infantry(InfantryInfo inf, Graphics g)
        {
            ShpReader InfShp = ShpReader.Load(General_File_String_From_Name(inf.Name));

            Bitmap TempBitmap = RenderUtils.RenderShp(InfShp, Pal,Frame_From_Infantry_Angle(inf.Angle));
            int subX, subY;
            Sub_Cell_Pixel_Offsets(inf.SubCell, out subX, out subY);

            g.DrawImage(TempBitmap, inf.X * CellSize + subX, inf.Y * CellSize + subY, TempBitmap.Width, TempBitmap.Height);
        }

        void Draw_Structures(Graphics g)
        {
            foreach (StructureInfo s in Structures)
            {
                Draw_Structure(s, g);
            }
        }

        void Draw_Structure(StructureInfo s, Graphics g)
        {
            ShpReader StructShp = ShpReader.Load(General_File_String_From_Name(s.Name));

            Bitmap StructBitmap = RenderUtils.RenderShp(StructShp, Pal, Frame_From_Building_HP(s));

            g.DrawImage(StructBitmap, s.X * CellSize, s.Y * CellSize, StructBitmap.Width, StructBitmap.Height);
        }

        void Draw_Bibs(Graphics g)
        {
            foreach (BibInfo bib in Bibs)
            {
                Draw_Bib(bib, g);
            }
        }

        void Draw_Bib(BibInfo bib, Graphics g)
        {
            ShpReader BibShp = ShpReader.Load(File_String_From_Name(bib.Name));
            int Frame = 0;

            int maxY = -1; int maxX = -1;
            switch (bib.Name.ToLower())
            {
                case "bib1": maxY = 2; maxX = 4; break;
                case "bib2": maxY = 2; maxX = 3; break;
                case "bib3": maxY = 2; maxX = 2; break;
                default: break;
            }

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Bitmap StructBitmap = RenderUtils.RenderShp(BibShp, Pal, Frame);

                    g.DrawImage(StructBitmap, (bib.X + x) * CellSize, (bib.Y + y) * CellSize, StructBitmap.Width, StructBitmap.Height);

                    Frame++;
                }
            }
        }

        void Draw_Template(CellStruct Cell, Graphics g, int X, int Y)
        {
//            if (Cell.Tile != 0 ) { return; }

            string TemplateString = TilesetsINI.getStringValue("TileSets", Cell.Template.ToString(), "0");

            TemplateReader Temp = TemplateReader.Load(File_String_From_Name(TemplateString));

            Bitmap TempBitmap = RenderUtils.RenderTemplate(Temp, Pal, Cell.Tile);
            g.DrawImage(TempBitmap, X * CellSize, Y * CellSize, TempBitmap.Width, TempBitmap.Height);
        }

        string File_String_From_Name(string Name)
        {
            return ("data/" + Theater + "/" + Name + TheaterFilesExtension);
        }

        string General_File_String_From_Name(string Name)
        {
            return ("data/general/" + Name + ".shp");
        }

        void Draw_Overlay(CellStruct Cell, Graphics g, int X, int Y)
        {
            string Overlay = Cell.Overlay;
            int Frame = 0;

            if (TiberiumStages.ContainsKey(Overlay))
            {
                Frame = -1;
                TiberiumStages.TryGetValue(Overlay, out Frame);
                int index = MapRandom.Next(1, 12); // creates a number between 1 and 12
                Overlay = string.Format("TI{0}", index);
            }

            ShpReader Shp = ShpReader.Load(File_String_From_Name(Overlay));

            Bitmap ShpBitmap = RenderUtils.RenderShp(Shp, Pal, Frame);
            g.DrawImage(ShpBitmap, X * CellSize, Y * CellSize, ShpBitmap.Width, ShpBitmap.Height);
        }

        void Draw_Terrain(CellStruct Cell, Graphics g, int X, int Y)
        {
            string[] TerrainData = Cell.Terrain.Split(',');

            ShpReader Shp = ShpReader.Load(File_String_From_Name(TerrainData[0]));

            Bitmap ShpBitmap = RenderUtils.RenderShp(Shp, Pal, 0);
            g.DrawImage(ShpBitmap, X * CellSize, Y * CellSize, ShpBitmap.Width, ShpBitmap.Height);
        }

        bool Is_Out_Of_Bounds(int X, int Y)
        {
            if (MapX > X || X >= MapX + MapWidth)
                return true;

            if (MapY > Y || Y >= MapY + MapHeight)
                return true;

            return false;
        }

        int Frame_From_Infantry_Angle(int Angle)
        {
            //            Console.WriteLine("Angle = {0}", Angle);

            if (Angle == 0) { return 0; }

            if (Angle > 224) { return 0; }
            if (Angle > 192) { return 1; }
            if (Angle > 160) { return 2; }
            if (Angle > 128) { return 3; }
            if (Angle > 96) { return 4; }
            if (Angle > 64) { return 5; }
            if (Angle > 32) { return 6; }
            if (Angle > 0) { return 7; }

            return -1;
        }

        int Frame_From_Unit_Angle(int Angle)
        {
            Console.WriteLine("Angle = {0}", Angle);

            if (Angle== 0) { return 0; }

            if (Angle > 248) { return 0; }
            if (Angle > 240) { return 1; }
            if (Angle > 232) { return 2; }
            if (Angle > 224) { return 3; }
            if (Angle > 216) { return 4; }
            if (Angle > 208) { return 5; }
            if (Angle > 200) { return 6; }
            if (Angle > 192) { return 7; }
            if (Angle > 184) { return 8; }
            if (Angle > 176) { return 9; }
            if (Angle > 168) { return 10; }
            if (Angle > 160) { return 11; }
            if (Angle > 152) { return 12; }
            if (Angle > 144) { return 13; }
            if (Angle > 136) { return 14; }
            if (Angle > 128) { return 15; }
            if (Angle > 120) { return 16; }
            if (Angle > 112) { return 17; }
            if (Angle > 104) { return 18; }
            if (Angle > 96) { return 19; }
            if (Angle > 88) { return 20; }
            if (Angle > 80) { return 21; }
            if (Angle > 72) { return 22; }
            if (Angle > 64) { return 23; }
            if (Angle > 56) { return 24; }
            if (Angle > 48) { return 25; }
            if (Angle > 40) { return 26; }
            if (Angle > 32) { return 27; }
            if (Angle > 24) { return 28; }
            if (Angle > 16) { return 29; }
            if (Angle > 8) { return 30; }
            if (Angle > 0) { return 31; }

            return -1;
        }
    }

    struct UnitInfo
    {
        public string Name;
        public string Side;
        public int Angle;
        public int X;
        public int Y;
    }

    struct InfantryInfo
    {
        public string Name;
        public string Side;
        public int Angle;
        public int X;
        public int Y;
        public int SubCell;
    }
    struct StructureInfo
    {
        public string Name;
        public string Side;
        public int Angle;
        public int X;
        public int Y;
        public int HP;
    }
    struct BibInfo
    {
        public string Name;
        public int X;
        public int Y;
    }
    struct BuildingBibInfo
    {
        public string Name;
        public int Yoffset;

        public BuildingBibInfo(string _Name, int _Yoffset)
        {
            Name = _Name;
            Yoffset = _Yoffset;
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
        public int Number;
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
