using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle creating the layout of a room
/// </summary>
public abstract class RoomLayout {

    /// <summary>
    /// Enum for a room's shape
    /// </summary>
    public enum RoomShape {
        Rectangular,
        TJunction,
        Donut
    }

    /// <summary>
    /// Enum for the type of corner cutting
    /// </summary>
    public enum CutCornerStyle {
        None,
        Straight,
        Jagged
    }

    /// <summary>
    /// Enum for the position of a corner
    /// </summary>
    public enum CornerPlacement {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    /// <summary>
    /// Enum for the style of layout of pillars ing a room
    /// </summary>
    public enum PillarStyle {
        None,
        Square,
        Rows
    }

    /// <summary>
    /// minimum and maximum sizes for a room
    /// </summary>
    private static int minX = 5, maxX = 7, minY = 5, maxY = 7;

    /// <summary>
    /// Width of the room
    /// </summary>
    public int XLength { get; private set; }

    /// <summary>
    /// Length of the room
    /// </summary>
    public int YLength { get; private set; }

    /// <summary>
    /// 2D array of roompieces that make up a room
    /// </summary>
    protected RoomPiece[,] layout;

    /// <summary>
    /// Style of the room
    /// </summary>
    protected RoomStyle.RoomStyleType style;

    /// <summary>
    /// Corner cut style fo the room
    /// </summary>
    protected CutCornerStyle corners;

    /// <summary>
    /// Width of the corners cut away
    /// </summary>
    protected int sizeOfCornersX;

    /// <summary>
    /// Length of the corners cut away
    /// </summary>
    protected int sizeOfCornersY;

    /// <summary>
    /// Array of which corners are to be cut
    /// </summary>
    protected CornerPlacement[] cornerPlacements;

    /// <summary>
    /// Minimum space left around a corner
    /// </summary>
    protected int minSpaceAroundDoor = 3;

    /// <summary>
    /// Minimum size of a corner
    /// </summary>
    protected int minCornerSize = 2;

    /// <summary>
    /// Gameobject holding all the room pieces
    /// </summary>
    protected GameObject room;

    /// <summary>
    /// Shape of the room
    /// </summary>
    protected RoomShape shape;

    /// <summary>
    /// Tyoe of the room
    /// </summary>
    protected Room.RoomType roomType;

    /// <summary>
    /// Minimum size of the room to have pillers
    /// </summary>
    private int minSizeForPillars = 6;

    /// <summary>
    /// Space between rooms in world space
    /// </summary>
    protected static int xPlacementIncrease = 100;

    /// <summary>
    /// Room position in world space
    /// </summary>
    protected static Vector3 roomPosition = new Vector3(0, 0, 0);

    /// <summary>
    /// Number of rooms created
    /// </summary>
    protected static int numberOfRooms = 0;

    /// <summary>
    /// Gameobject used for the minimap for this room
    /// </summary>
    protected GameObject mapGameObject;

    /// <summary>
    /// Initialises one starting a new level
    /// </summary>
    public static void NewLevel() {
        roomPosition = new Vector3(0, 0, 0);
        numberOfRooms = 0;
    }

    /// <summary>
    /// Static function to create a random room layout. Anything after MapGO can be set to -1 for random.
    /// Sets any random variables, then picks a type of roomlayout anc creates it
    /// </summary>
    /// <param name="doors">Number of doors</param>
    /// <param name="type">Type of room</param>
    /// <param name="mapGO">Minimap gameobject for the room</param>
    /// <param name="xSize">Width of the room</param>
    /// <param name="ySize">Length of the room</param>
    /// <param name="style">Style of the room</param>
    /// <param name="cornerStyle">style of any cut corners</param>
    /// <param name="numberOfCornersToCut">number of corners to cut</param>
    /// <param name="cornerSizeX">width of the cut corners</param>
    /// <param name="cornerSizeY">length of the cut corners</param>
    /// <returns>The room layout</returns>
    public static RoomLayout GetRandomRoomLayout(List<Room.Door> doors, Room.RoomType type, GameObject mapGO, int xSize = -1, int ySize = -1, RoomStyle.RoomStyleType style = (RoomStyle.RoomStyleType)(-1),
        CutCornerStyle cornerStyle = (CutCornerStyle)(-1), int numberOfCornersToCut = -1,
        int cornerSizeX = -1, int cornerSizeY = -1) {

        xSize = (xSize == -1) ? UnityEngine.Random.Range(minX, maxX + 1) : xSize;
        ySize = (ySize == -1) ? UnityEngine.Random.Range(minY, maxY + 1) : ySize;

        xSize = Mathf.Clamp(xSize, minX, maxX);
        ySize = Mathf.Clamp(ySize, minY, maxY);
        
        RoomShape roomShape = EnumExtensions.GetRandomEntry<RoomShape>();
        RoomLayout layout;
        switch (roomShape) {
            case RoomShape.Rectangular:
                if (UnityEngine.Random.Range(1, 3) == 1) {
                    layout = new RectangularRoomLayout(doors, type, mapGO, xSize, ySize, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY);
                    return layout;
                } else {
                    layout = new SquareRoomLayout(doors, type, mapGO, xSize, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY);
                    return layout;
                }
            case RoomShape.TJunction:
                layout = new TRoomLayout(doors, type, mapGO, xSize, ySize, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY);
                return layout;
            case RoomShape.Donut:
                layout = new DonutLayout(doors, type, mapGO, xSize, ySize, style);
                return layout;
            default:
                break;
        }

        return new RectangularRoomLayout(doors, type, mapGO, xSize, ySize, style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY);

    }

    /// <summary>
    /// Creates the boss room from the prefab
    /// </summary>
    /// <param name="doors"></param>
    /// <param name="mapGO"></param>
    public static void CreateBossRoom(List<Room.Door> doors, GameObject mapGO) {
        GameObject prefab = AllRoomStyles.GetRoomStyle(RoomStyle.RoomStyleType.Arcade).BossRoomPrefab;
        GameObject room = GameObject.Instantiate(prefab, roomPosition, Quaternion.identity);
        room.AddComponent<ExplodeRoom>();
        roomPosition.x += xPlacementIncrease;
        room.transform.Rotate(new Vector3(0, 90 * (int)doors[0].Direction, 0));
        if(doors[0].Direction == Room.Direction.Up || doors[0].Direction == Room.Direction.Down) {
            room.transform.Rotate(new Vector3(0, 180, 0));
        }
        RoomNavmesh navmesh = room.AddComponent<RoomNavmesh>();
        Teleporter door = room.GetComponentInChildren<Teleporter>();
        room.AddComponent<BossSpawner>();
        if (door) {
            door.ThisDoor = doors[0];
            door.roomNavmesh = navmesh;
        }
        RoomInformation information = room.GetComponent<RoomInformation>();
        information.Style = AllRoomStyles.GetStyle(EnumExtensions.GetRandomEntry<RoomStyle.RoomStyleType>());
        information.MapObject = mapGO;
        information.centrePoint = information.CentrePiece.transform.position;


    }

    /// <summary>
    /// Adds pillars to a room
    /// </summary>
    private void AddPillars() {
        PillarStyle pillarStyle = EnumExtensions.GetRandomEntry<PillarStyle>();
        switch (pillarStyle) {
            case PillarStyle.None:
                return;
            case PillarStyle.Square:
                PlacePillarsSquare();
                return;
            case PillarStyle.Rows:
                PlacePillarsRows();
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// Places pillars in 2 romw
    /// </summary>
    private void PlacePillarsRows() {
        if(layout.GetLength(0) >= layout.GetLength(1)){
            int space = 2;
            int topY = (layout.GetLength(1) - 2) / 3;
            int bottomY = layout.GetLength(1) - topY - 1;

            for(int x = space; x < layout.GetLength(0) - 2; x+= space) {
                if(layout[x, topY] != null && layout[x, topY].PieceType == RoomPiece.RoomPieceType.floor
                    && !CheckForNeighbourType(x, topY, RoomPiece.RoomPieceType.innerCorner)) {
                    layout[x, topY].PieceType = RoomPiece.RoomPieceType.pillar;
                    layout[x, topY].PieceRotation = RoomPiece.Rotation.Down;
                }
                if (layout[x, bottomY] != null && layout[x, bottomY].PieceType == RoomPiece.RoomPieceType.floor
                    && !CheckForNeighbourType(x, bottomY, RoomPiece.RoomPieceType.innerCorner)) {
                    layout[x, bottomY].PieceType = RoomPiece.RoomPieceType.pillar;
                    layout[x, bottomY].PieceRotation = RoomPiece.Rotation.Left;
                }
            }
        } else {
            int space = UnityEngine.Random.Range(2, 4);
            int leftX = (layout.GetLength(0) - 2) / 3;
            int rightX = layout.GetLength(0) - leftX - 1;

            for (int y = space; y < layout.GetLength(1) - 2; y += space) {
                if(layout[leftX, y] != null && layout[leftX, y].PieceType == RoomPiece.RoomPieceType.floor
                    && !CheckForNeighbourType(leftX, y, RoomPiece.RoomPieceType.innerCorner)) {
                    layout[leftX, y].PieceType = RoomPiece.RoomPieceType.pillar;
                    layout[leftX, y].PieceRotation = RoomPiece.Rotation.Down;
                }
                if(layout[rightX, y] != null && layout[rightX, y].PieceType == RoomPiece.RoomPieceType.floor
                    && !CheckForNeighbourType(rightX, y, RoomPiece.RoomPieceType.innerCorner)) {
                    layout[rightX, y].PieceType = RoomPiece.RoomPieceType.pillar;
                    layout[rightX, y].PieceRotation = RoomPiece.Rotation.Right;
                }
            }
        }
    }

    /// <summary>
    /// Places pillars in a square
    /// </summary>
    private void PlacePillarsSquare() {
        if(layout.GetLength(0) < minSizeForPillars || layout.GetLength(1) < minSizeForPillars) {
            return;
        }
        int xDistance = (layout.GetLength(0) - 2) / 3;
        int yDistance = (layout.GetLength(1) - 2) / 3;

        if(layout[xDistance, yDistance] != null && layout[xDistance,yDistance].PieceType == RoomPiece.RoomPieceType.floor
            && !CheckForNeighbourType(xDistance, yDistance, RoomPiece.RoomPieceType.innerCorner)) {
            layout[xDistance, yDistance].PieceType = RoomPiece.RoomPieceType.pillar;
            layout[xDistance, yDistance].PieceRotation = RoomPiece.Rotation.Down;
        }

        if(layout[layout.GetLength(0) - xDistance - 1, yDistance] != null && layout[layout.GetLength(0) - xDistance - 1, yDistance].PieceType == RoomPiece.RoomPieceType.floor
            && !CheckForNeighbourType(layout.GetLength(0) - xDistance - 1, yDistance, RoomPiece.RoomPieceType.innerCorner)) {
            layout[layout.GetLength(0) - xDistance - 1, yDistance].PieceType = RoomPiece.RoomPieceType.pillar;
            layout[layout.GetLength(0) - xDistance - 1, yDistance].PieceRotation = RoomPiece.Rotation.Right;
        }

        if (layout[xDistance, layout.GetLength(1) - yDistance - 1] != null && layout[xDistance, layout.GetLength(1) - yDistance - 1].PieceType == RoomPiece.RoomPieceType.floor
            && !CheckForNeighbourType(xDistance, layout.GetLength(1) - yDistance - 1, RoomPiece.RoomPieceType.innerCorner)) {
            layout[xDistance, layout.GetLength(1) - yDistance - 1].PieceType = RoomPiece.RoomPieceType.pillar;
            layout[xDistance, layout.GetLength(1) - yDistance - 1].PieceRotation = RoomPiece.Rotation.Left;
        }

        if (layout[layout.GetLength(0) - xDistance - 1, layout.GetLength(1) - yDistance - 1] != null && layout[layout.GetLength(0) - xDistance - 1, layout.GetLength(1) - yDistance - 1].PieceType == RoomPiece.RoomPieceType.floor
            && !CheckForNeighbourType(layout.GetLength(0) - xDistance - 1, layout.GetLength(1) - yDistance - 1, RoomPiece.RoomPieceType.innerCorner)) {
            layout[layout.GetLength(0) - xDistance - 1, layout.GetLength(1) - yDistance - 1].PieceType = RoomPiece.RoomPieceType.pillar;
            layout[layout.GetLength(0) - xDistance - 1, layout.GetLength(1) - yDistance - 1].PieceRotation = RoomPiece.Rotation.Up;
        }
    }

    /// <summary>
    /// Called when all rooms are created. connects up the doors and sets the path length on the enemy spawner
    /// </summary>
    /// <param name="pathLength"></param>
    public static void FinalizeLevel(int pathLength) {
        Teleporter.ConnectDoors();
        EnemySpawner.NumberOfRoomsOnMainPath = pathLength;
    }

    /// <summary>
    /// sets the sizes and initialises the array of room pieces
    /// </summary>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    protected virtual void SetSizes(int xSize, int ySize) {
        XLength = xSize;
        YLength = ySize;
        layout = new RoomPiece[XLength, YLength];
    }

    /// <summary>
    /// Sets the different types, using random ones where none supplied
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cornerStyle"></param>
    /// <param name="numberOfCornersToCut"></param>
    /// <param name="cornerSizeX"></param>
    /// <param name="cornerSizeY"></param>
    protected virtual void SetTypes(RoomStyle.RoomStyleType type, CutCornerStyle cornerStyle, int numberOfCornersToCut = -1,
        int cornerSizeX = -1, int cornerSizeY = -1) {
        if ((int)type == (-1)) {
            type = EnumExtensions.GetRandomEntry<RoomStyle.RoomStyleType>();
        }
        style = type;
        if ((int)cornerStyle == (-1)) {
            cornerStyle = EnumExtensions.GetRandomEntry<CutCornerStyle>();
        }

        if ((XLength - minSpaceAroundDoor) / 2 < minCornerSize || (YLength - minSpaceAroundDoor) / 2 < minCornerSize) {
            cornerStyle = CutCornerStyle.None;
        }

        corners = cornerStyle;

        if (numberOfCornersToCut == -1 || numberOfCornersToCut > 4) {
            numberOfCornersToCut = UnityEngine.Random.Range(0, 5);
        }

        cornerPlacements = EnumExtensions.GetDistinctRandomEntries<CornerPlacement>(numberOfCornersToCut);

        sizeOfCornersX = (cornerSizeX == -1) ? UnityEngine.Random.Range(minCornerSize, (XLength - minSpaceAroundDoor) / 2) : cornerSizeX;
        sizeOfCornersY = (cornerSizeY == -1) ? UnityEngine.Random.Range(minCornerSize, (YLength - minSpaceAroundDoor) / 2) : cornerSizeX;
    }


    /// <summary>
    /// Base constructor. Parameters as for static function. Sets everything up then creates the room
    /// </summary>
    /// <param name="doors"></param>
    /// <param name="type"></param>
    /// <param name="mapGO"></param>
    /// <param name="xSize"></param>
    /// <param name="ySize"></param>
    /// <param name="style"></param>
    /// <param name="cornerStyle"></param>
    /// <param name="numberOfCornersToCut"></param>
    /// <param name="cornerSizeX"></param>
    /// <param name="cornerSizeY"></param>
    public RoomLayout(List<Room.Door> doors, Room.RoomType type, GameObject mapGO ,int xSize, int ySize, RoomStyle.RoomStyleType style = (RoomStyle.RoomStyleType)(-1) , 
        CutCornerStyle cornerStyle = (CutCornerStyle)(-1), int numberOfCornersToCut = -1,
        int cornerSizeX = -1, int cornerSizeY = -1) {

        mapGameObject = mapGO;
        numberOfRooms++;
        roomType = type;
        SetSizes(xSize, ySize);
        SetTypes(style, cornerStyle, numberOfCornersToCut, cornerSizeX, cornerSizeY);

        SetupLayout();
        CutCorners();
        PlaceDoors(doors);
        CheckForCorridors();
        AddPillars();
        GenerateRoom();

    }

    /// <summary>
    /// checks whether the cutting of corners has led to paths one tile wide, and if so deal with potential problems that causes
    /// </summary>
    private void CheckForCorridors() {

        for(int x = 1; x < XLength - 2; x++) {
            if(layout[x - 1, 0] == null && layout[x + 1, 0] == null && layout[x,0] != null) {
                if(layout[x, 0].PieceType == RoomPiece.RoomPieceType.wall || layout[x, 0].PieceType == RoomPiece.RoomPieceType.corner) {
                    layout[x, 0] = new RoomPiece(RoomPiece.RoomPieceType.corridorEnd, RoomPiece.Rotation.Left, room ,layout[x, 0].Position / RoomPiece.ModelScale, layout[x, 0].PieceStyle);
                    int y = 1;
                    while(layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x - 1, y] = null;
                        layout[x + 1, y] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, RoomPiece.Rotation.Left, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        y++;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x, y + 1].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                } else if(layout[x, 0].PieceType == RoomPiece.RoomPieceType.door || layout[x, 0].PieceType == RoomPiece.RoomPieceType.cornerDoorLeft || layout[x, 0].PieceType == RoomPiece.RoomPieceType.cornerDoorRight) {
                    DoorPiece doorPiece = layout[x, 0] as DoorPiece;
                    layout[x, 0] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, layout[x, 0].PieceRotation, room, layout[x, 0].Position / RoomPiece.ModelScale, layout[x, 0].PieceStyle, doorPiece.ThisDoor);
                    int y = 1;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x - 1, y] = null;
                        layout[x + 1, y] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, layout[x, y].PieceRotation, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        y++;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x, y + 1].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                }
                

            }
            if (layout[x - 1, YLength - 1] == null && layout[x + 1, YLength - 1] == null && layout[x, YLength - 1] != null) {
                if (layout[x, YLength - 1].PieceType == RoomPiece.RoomPieceType.wall || layout[x, YLength - 1].PieceType == RoomPiece.RoomPieceType.corner) {
                    layout[x, YLength - 1] = new RoomPiece(RoomPiece.RoomPieceType.corridorEnd, RoomPiece.Rotation.Right, room, layout[x, YLength - 1].Position / RoomPiece.ModelScale, layout[x, YLength - 1].PieceStyle);
                    int y = YLength - 2;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x - 1, y] = null;
                        layout[x + 1, y] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, RoomPiece.Rotation.Right, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        y--;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x, y - 1].PieceType = RoomPiece.RoomPieceType.emptyFloor;

                } else if (layout[x, YLength - 1].PieceType == RoomPiece.RoomPieceType.door || layout[x, YLength - 1].PieceType == RoomPiece.RoomPieceType.cornerDoorLeft || layout[x, YLength - 1].PieceType == RoomPiece.RoomPieceType.cornerDoorRight) {
                    DoorPiece doorPiece = layout[x, YLength - 1] as DoorPiece;
                    layout[x, YLength - 1] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, layout[x, YLength - 1].PieceRotation, room, layout[x, YLength - 1].Position / RoomPiece.ModelScale, layout[x, YLength - 1].PieceStyle, doorPiece.ThisDoor);
                    int y = YLength - 2;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x - 1, y] = null;
                        layout[x + 1, y] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, layout[x, y].PieceRotation, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        y--;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x, y - 1].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                }
            }
        }

        for(int y = 1; y < YLength - 2; y++) {
            if (layout[0, y - 1] == null && layout[0, y + 1] == null && layout[0, y] != null) {
                if (layout[0, y].PieceType == RoomPiece.RoomPieceType.wall || layout[0, y].PieceType == RoomPiece.RoomPieceType.corner) {
                    Debug.Log("0,y corridor end in room " + numberOfRooms);
                    Debug.Log(layout[0, y].PieceRotation);
                    layout[0, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorEnd, layout[0, y].PieceRotation, room, layout[0, y].Position / RoomPiece.ModelScale, layout[0, y].PieceStyle);
                    int x = 1;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x, y - 1] = null;
                        layout[x, y + 1] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, layout[x, y].PieceRotation, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        x++;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x + 1, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;

                } else if (layout[0, y].PieceType == RoomPiece.RoomPieceType.door || layout[0, y].PieceType == RoomPiece.RoomPieceType.cornerDoorLeft || layout[0, y].PieceType == RoomPiece.RoomPieceType.cornerDoorRight) {
                    DoorPiece doorPiece = layout[0, y] as DoorPiece;
                    layout[0, y] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, layout[0, y].PieceRotation, room, layout[0, y].Position / RoomPiece.ModelScale, layout[0, y].PieceStyle, doorPiece.ThisDoor);
                    int x = 1;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x, y - 1] = null;
                        layout[x, y + 1] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, layout[x, y].PieceRotation, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        x++;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x + 1, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                }
            }

            if (layout[XLength - 1, y - 1] == null && layout[XLength - 1, y + 1] == null && layout[XLength - 1, y] != null) {
                if (layout[XLength - 1, y].PieceType == RoomPiece.RoomPieceType.wall || layout[XLength - 1, y].PieceType == RoomPiece.RoomPieceType.corner) {
                    Debug.Log("xlength - 1, y corridor end in room " + numberOfRooms);
                    Debug.Log(layout[XLength - 1, y].PieceRotation);
                    layout[XLength - 1, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorEnd, layout[XLength - 1, y].PieceRotation, room, layout[XLength - 1, y].Position / RoomPiece.ModelScale, layout[XLength - 1, y].PieceStyle);
                    int x = XLength - 2;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x, y - 1] = null;
                        layout[x, y + 1] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, layout[x, y].PieceRotation, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        x--;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x - 1, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                } else if (layout[XLength - 1, y].PieceType == RoomPiece.RoomPieceType.door || layout[XLength - 1, y].PieceType == RoomPiece.RoomPieceType.cornerDoorLeft || layout[XLength - 1, y].PieceType == RoomPiece.RoomPieceType.cornerDoorRight) {
                    DoorPiece doorPiece = layout[XLength - 1, y] as DoorPiece;
                    layout[XLength - 1, y] = new DoorPiece(RoomPiece.RoomPieceType.corridorEndDoor, layout[XLength - 1, y].PieceRotation, room, layout[XLength - 1, y].Position / RoomPiece.ModelScale, layout[XLength - 1, y].PieceStyle, doorPiece.ThisDoor);
                    int x = XLength - 2;
                    while (layout[x, y].PieceType != RoomPiece.RoomPieceType.floor) {
                        layout[x, y - 1] = null;
                        layout[x, y + 1] = null;
                        layout[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corridorPiece, layout[x, y].PieceRotation, room, layout[x, y].Position / RoomPiece.ModelScale, layout[x, y].PieceStyle);
                        x--;
                    }
                    layout[x, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                    layout[x - 1, y].PieceType = RoomPiece.RoomPieceType.emptyFloor;
                }
            }
        }
    }

    /// <summary>
    /// Creates the room's gameobject, adds any componants needed and sets them up. Calls Generator.GenerateRoom to
    /// turn the roompieces into gameobjects and deals with setuo if the room is the starting room
    /// </summary>
    private void GenerateRoom() {
        GameObject room = new GameObject("Room " + numberOfRooms.ToString());
        room.transform.position = roomPosition;
        roomPosition.x += xPlacementIncrease;
        RoomInformation information = room.AddComponent<RoomInformation>();
        information.XSize = layout.GetLength(0) + 1;
        information.ZSize = layout.GetLength(1) + 1;
        information.Type = roomType;
        information.MapObject = mapGameObject;
        information.Style = AllRoomStyles.GetStyle(style);
        RoomNavmesh roomNavmesh = room.AddComponent<RoomNavmesh>();
        RoomGenerator generator = room.AddComponent<RoomGenerator>();
        EnemySpawner enemySpawner = room.AddComponent<EnemySpawner>();
        if(roomType == Room.RoomType.Starting) {
            room.AddComponent<ExplodeRoom>();
            room.tag = "Starting Room";
        }
        generator.GenerateRoom(layout);
        foreach(Teleporter door in room.GetComponentsInChildren<Teleporter>()) {
            door.Room = room;
            door.roomNavmesh = roomNavmesh;
        }

        if(roomType == Room.RoomType.Starting) {
            int xpos = 0;
            int ypos = 0;
            bool found = false;
            int xSearch = 1;
            int ySearch = 1;
            int searchPattern = 0;
            while (!found && searchPattern < 4) {
                xpos = layout.GetLength(0) / 2 + 1;
                ypos = layout.GetLength(1) / 2 + 1; 
                for (; xpos < layout.GetLength(0) && xpos >= 0; xpos += xSearch) {
                    for (; ypos < layout.GetLength(1) && ypos >= 0; ypos += ySearch) {
                        if (layout[xpos, ypos] != null && layout[xpos, ypos].PieceType != RoomPiece.RoomPieceType.block && layout[xpos, ypos].PieceType != RoomPiece.RoomPieceType.innerCorner) {
                            found = true;
                            break;
                        }
                    }
                    if (found) {
                        break;
                    }
                }
                if (!found) {
                    searchPattern++;
                    switch (searchPattern) {
                        case 1:
                            xSearch = 1;
                            ySearch = -1;
                            break;
                        case 2:
                            xSearch = -1;
                            ySearch = 1;
                            break;
                        case 3:
                            xSearch = -1;
                            ySearch = -1;
                            break;
                        default:
                            break;
                    }
                }
            }
            Vector3 startPos = generator.GetPiecePosition(xpos, ypos);
            PlayerSpawner.instance.SpawnPlayer(startPos);
            enemySpawner.SpawnEnemies();
            MapCreator.EnterRoom(mapGameObject);
        }


    }

    /// <summary>
    /// Handles the different types of corner cutting
    /// </summary>
    private void CutCorners() {
        switch (corners) {
            case CutCornerStyle.None:
                return;
            case CutCornerStyle.Straight:
                CreateStraightCorners();
                return;
            case CutCornerStyle.Jagged:
                CreateStraightCorners();
                MakeCornersJagged();
                return;
            default:
                return;
        }
    }

    /// <summary>
    /// Creates striaght cut corners
    /// </summary>
    private void CreateStraightCorners() {
        RoomPiece[,] corner = CreateStraightTopLeftCorner();
        foreach (CornerPlacement cornerPlacement in cornerPlacements) {
            ChangeCorner(corner, cornerPlacement);
        }
    }

    /// <summary>
    /// Creates the corners and places them in the right place in the layout array
    /// </summary>
    /// <param name="topLeftCorner"></param>
    /// <param name="cornerPlacement"></param>
    private void ChangeCorner(RoomPiece[,] topLeftCorner, CornerPlacement cornerPlacement) {

        switch (cornerPlacement) {
            case CornerPlacement.TopLeft:
                PlaceCorner(CreateStraightTopLeftCorner(), 0, 0);
                break;
            case CornerPlacement.TopRight:
                PlaceCorner(CreateStraightTopRightCorner(), XLength - topLeftCorner.GetLength(0), 0); ;
                break;
            case CornerPlacement.BottomLeft:
                PlaceCorner(CreateStraightBottomLeftCorner(), 0, YLength - topLeftCorner.GetLength(1));
                break;
            case CornerPlacement.BottomRight:
                PlaceCorner(CreateStraightBottomRightCorner(), XLength - topLeftCorner.GetLength(0), YLength - topLeftCorner.GetLength(1));
                break;
            default:
                PlaceCorner(topLeftCorner, 0, 0);
                break;
        }
        RemoveUnnecessaryFloor(cornerPlacement);
    }

    /// <summary>
    /// Removes unnecessary floor pieces after the corners are cut
    /// </summary>
    /// <param name="cornerPlacement"></param>
    private void RemoveUnnecessaryFloor(CornerPlacement cornerPlacement) {
        switch (cornerPlacement) {
            case CornerPlacement.TopLeft:
                for (int x = 0; x < sizeOfCornersX; x++) {
                    for (int y = 0; y < sizeOfCornersY; y++) {
                        if (layout[x, y].PieceType == RoomPiece.RoomPieceType.floor) {
                            layout[x, y] = null;
                        }
                    }
                }
                break;
            case CornerPlacement.TopRight:
                for (int x = layout.GetLength(0) - sizeOfCornersX; x < layout.GetLength(0); x++) {
                    for (int y = 0; y < sizeOfCornersY; y++) {
                        if (layout[x, y].PieceType == RoomPiece.RoomPieceType.floor) {
                            layout[x, y] = null;
                        }
                    }
                }
                break;
            case CornerPlacement.BottomLeft:
                for (int x = 0; x < sizeOfCornersX; x++) {
                    for (int y = layout.GetLength(1) - sizeOfCornersY; y < layout.GetLength(1); y++) {
                        if (layout[x, y].PieceType == RoomPiece.RoomPieceType.floor) {
                            layout[x, y] = null;
                        }
                    }
                }
                break;
            case CornerPlacement.BottomRight:
                for (int x = layout.GetLength(0) - sizeOfCornersX; x < layout.GetLength(0); x++) {
                    for (int y = layout.GetLength(1) - sizeOfCornersY; y < layout.GetLength(1); y++) {
                        if (layout[x, y].PieceType == RoomPiece.RoomPieceType.floor) {
                            layout[x, y] = null;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Places the cut corner in the layout arran
    /// </summary>
    /// <param name="cornerArray"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    private void PlaceCorner(RoomPiece[,] cornerArray, int startX, int startY) {
        for(int xPos = startX, x = 0; xPos <= layout.GetLength(0) && x < cornerArray.GetLength(0); xPos++, x++) {
            for(int yPos = startY, y = 0; yPos <= layout.GetLength(1) && y < cornerArray.GetLength(1); yPos++, y++) {
                layout[xPos, yPos] = cornerArray[x, y];
                if (layout[xPos, yPos] != null) {
                    layout[xPos, yPos].Position = new Vector3(xPos, 0, yPos);
                }
            }
        }

    }

    /// <summary>
    /// Makes the corners jagged
    /// </summary>
    private void MakeCornersJagged() {
        for(int x = 0; x < XLength; x++) {
            for(int y = 0; y < YLength; y++) {
                RoomPiece piece = layout[x, y];
                if (piece != null && piece.PieceType == RoomPiece.RoomPieceType.innerCorner) {
                    layout[x,y] = new RoomPiece(RoomPiece.RoomPieceType.corner, piece.PieceRotation, room, piece.Position / RoomPiece.ModelScale, style);
                }
            }
        }
    }

    /// <summary>
    /// Creates the layout for a cut corner in the top left of a room
    /// </summary>
    /// <returns>The corner piece array</returns>
    private RoomPiece[,] CreateStraightTopLeftCorner() {
        RoomPiece[,] cornerPieces = new RoomPiece[sizeOfCornersX + 1, sizeOfCornersY + 1];
        for(int x = 0; x <= sizeOfCornersX; x++) {
            for(int y = 0; y <= sizeOfCornersY; y++) {
                if (x == sizeOfCornersX - 1 && y == sizeOfCornersY - 1) { // Will be inverted corner piece
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.innerCorner, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == 0 && y == sizeOfCornersY) || x == sizeOfCornersX && y == 0) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if((x == sizeOfCornersX && y == sizeOfCornersY - 1) || x == sizeOfCornersX - 1 && y == sizeOfCornersY) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if(x == sizeOfCornersX && y < sizeOfCornersY - 1) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if(x < sizeOfCornersX - 1 && y == sizeOfCornersY) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
                    continue;
                }

                cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
            }
        }
        return cornerPieces;
    }

    /// <summary>
    /// Creates the layout for a cut corner in the top right of a room
    /// </summary>
    /// <returns>The corner piece array</returns>
    private RoomPiece[,] CreateStraightTopRightCorner() {
        RoomPiece[,] cornerPieces = new RoomPiece[sizeOfCornersX + 1, sizeOfCornersY + 1];
        for (int x = 0; x <= sizeOfCornersX; x++) {
            for (int y = 0; y <= sizeOfCornersY; y++) {
                if (x == 1 && y == sizeOfCornersY - 1) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.innerCorner, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == sizeOfCornersX && y == sizeOfCornersY) || x == 0 && y == 0) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == 0 && y == sizeOfCornersY - 1) || x == 11 && y == sizeOfCornersY) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if (x == 0 && y < sizeOfCornersY - 1) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Down, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if (x > 1 && y == sizeOfCornersY) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Left, room, new Vector3(x, 0, y), style);
                    continue;
                }

                cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
            }
        }
        return cornerPieces;
    }

    /// <summary>
    /// Creates the layout for a cut corner in the bottom left of a room
    /// </summary>
    /// <returns>The corner piece array</returns>
    private RoomPiece[,] CreateStraightBottomLeftCorner() {
        RoomPiece[,] cornerPieces = new RoomPiece[sizeOfCornersX + 1, sizeOfCornersY + 1];
        for (int x = 0; x <= sizeOfCornersX; x++) {
            for (int y = 0; y <= sizeOfCornersY; y++) {
                if (x == sizeOfCornersX - 1 && y == 1) { // Will be inverted corner piece
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.innerCorner, RoomPiece.Rotation.Right, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == 0 && y == 0) || x == sizeOfCornersX && y == sizeOfCornersY) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Right, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == sizeOfCornersX && y == 1) || x == sizeOfCornersX - 1 && y == 0) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if (x == sizeOfCornersX && y > 1) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if (x < sizeOfCornersX - 1 && y == 0) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Right, room, new Vector3(x, 0, y), style);
                    continue;
                }

                cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
            }
        }
        return cornerPieces;
    }

    /// <summary>
    /// Creates the layout for a cut corner in the bottom right of a room
    /// </summary>
    /// <returns>The corner piece array</returns>
    private RoomPiece[,] CreateStraightBottomRightCorner() {
        RoomPiece[,] cornerPieces = new RoomPiece[sizeOfCornersX + 1, sizeOfCornersY + 1];
        for (int x = 0; x <= sizeOfCornersX; x++) {
            for (int y = 0; y <= sizeOfCornersY; y++) {
                if (x == 1 && y == 1) { // Will be inverted corner piece
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.innerCorner, RoomPiece.Rotation.Down, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == sizeOfCornersX && y == 0) || x == 0 && y == sizeOfCornersY) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.corner, RoomPiece.Rotation.Down, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if ((x == 0 && y == 1) || x == 1 && y == 0) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
                    continue;
                }
                if (x == 0 && y > 1) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Down, room, new Vector3(x, 0, y), style);
                    continue;
                }

                if (x > 1 && y == 0) {
                    cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.wall, RoomPiece.Rotation.Right, room, new Vector3(x, 0, y), style);
                    continue;
                }

                cornerPieces[x, y] = new RoomPiece(RoomPiece.RoomPieceType.floor, RoomPiece.Rotation.Up, room, new Vector3(x, 0, y), style);
            }
        }
        return cornerPieces;
    }


    /// <summary>
    /// checks whether any of the piece types supplied are in neighbouring squares 
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <param name="pieceTypes"></param>
    /// <returns></returns>
    private bool CheckForNeighbourType(int xPos, int yPos, params RoomPiece.RoomPieceType[] pieceTypes) {

        for(int x = -1; x < 2; x++) {
            for(int y = -1; y < 2; y++) {
                if(layout[xPos + x, yPos + y] == null) {
                    continue;
                }
                if(pieceTypes.Contains(layout[xPos + x, yPos + y].PieceType)) {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// abstract class to start layout
    /// </summary>
    protected abstract void SetupLayout();

    /// <summary>
    /// abstract class to place doors
    /// </summary>
    /// <param name="doors"></param>
    protected abstract void PlaceDoors(List<Room.Door> doors);

}
