using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Extention methods for Direction Enum
/// </summary>
public static class DirectionExtensions {

    /// <summary>
    /// Returns the opposite direction to the current one
    /// </summary>
    /// <param name="direction">Current direction</param>
    /// <returns></returns>
    public static Room.Direction GetOppositeDirection(this Room.Direction direction) {
        if(direction == Room.Direction.Count) {
            return Room.Direction.Count;
        }
        return (Room.Direction)(((int)direction + ((int)Room.Direction.Count / 2)) % (int)Room.Direction.Count);
    }

    /// <summary>
    /// Gets the next vector 2 position given the current position and direction
    /// </summary>
    /// <param name="direction">The current direction</param>
    /// <param name="currentPosition">The current position</param>
    /// <returns></returns>
    public static Vector2 GetNextPosition(this Room.Direction direction, Vector2 currentPosition) {
        Vector2 nextPosition = currentPosition;
        switch(direction) {
            case Room.Direction.Up:
                nextPosition.y += 1f;
                break;
            case Room.Direction.Right:
                nextPosition.x += 1f;
                break;
            case Room.Direction.Down:
                nextPosition.y -= 1f;
                break;
            case Room.Direction.Left:
                nextPosition.x -= 1f;
                break;
        }
        return nextPosition;
    }
}

/// <summary>
/// Class to hold data about a room. Static functions handle generating the room positions and connections between rooms
/// </summary>
public class Room 
{
    /// <summary>
    /// Enum to represent a direction
    /// </summary>
    public enum Direction {
        Up = 0,
        Right,
        Down,
        Left,
        Count
    }

    /// <summary>
    /// enum to represent the types of room
    /// </summary>
    public enum RoomType {
        Starting = 0,
        MainPath,
        Challange,
        Boss, 
    }

    /// <summary>
    /// Class to hold dara about a door in a room
    /// </summary>
    public class Door {

        /// <summary>
        /// The number of doors created
        /// </summary>
        private static int numberOfDoors;

        /// <summary>
        /// ID to identiffy a door
        /// </summary>
        private int doorID;

        /// <summary>
        /// Direction the door goes in
        /// </summary>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Room the door is in
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="direction"></param>
        public Door(Direction direction) {
            numberOfDoors++;
            doorID = numberOfDoors;
            Direction = direction;
        }

        /// <summary>
        /// Allows comparisons of doors
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if(obj == null || !GetType().Equals(obj.GetType())) {
                return false;
            }
            Door objDoor = (Door)obj;
            return objDoor.doorID == doorID;
        }

        /// <summary>
        /// Gets the corresponding door in the next room
        /// </summary>
        /// <returns></returns>
        public Door GetConnectedDoor() {
            return GetDoorInDirection(Room, Direction.GetOppositeDirection());
        }

        /// <summary>
        /// Gets the type of room the door is connected to
        /// </summary>
        /// <returns></returns>
        public RoomType GetConnectedRoomType() {
            return Room.roomType;
        }

        /// <summary>
        /// Allows doors to be used as keys in a dictionary
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return doorID;
        }
    }

    /// <summary>
    /// List of the doors in the room
    /// </summary>
    private List<Door> doors;

    /// <summary>
    /// The type of the room
    /// </summary>
    private RoomType roomTypeField;
    public RoomType roomType { get { return roomTypeField; } 
        private set { 
            roomTypeField = value;
        }
    }

    /// <summary>
    /// The gameobject for the room in the minimap
    /// </summary>
    public GameObject mapGO { get; private set; }

    //Getter for number of doors
    public int NumberOfDoors { get { return doors.Count; } }

    /// <summary>
    /// Whether the room is on the main path
    /// </summary>
    public bool IsOnMainPath { get; set; }

    /// <summary>
    /// Position of the room in relation to the other rooms
    /// </summary>
    private Vector2 position;
    public Vector2 Position { get { return position; } }

    /// <summary>
    /// List of all the rooms
    /// </summary>
    private static List<Room> allRooms;
    public static List<Room> AllRooms { get { return allRooms; } }

    /// <summary>
    /// List of the deadend rooms
    /// </summary>
    private static List<Room> deadEndRooms;

    /// <summary>
    /// List of rooms with 2 or more doors
    /// </summary>
    private static List<Room> openRooms;
    private static List<Vector2> positions;

    /// <summary>
    /// Creates rooms of all possible types to use in placement
    /// </summary>
    private static void CreateAllRooms() {
        if(allRooms != null) {
            return;
        }
        allRooms = new List<Room>();
        allRooms.Add(new Room(new Door(Direction.Up)));
        allRooms.Add(new Room(new Door(Direction.Right)));
        allRooms.Add(new Room(new Door(Direction.Down)));
        allRooms.Add(new Room(new Door(Direction.Left)));

        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Right)));
        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Down)));
        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Left)));
        allRooms.Add(new Room(new Door(Direction.Right), new Door(Direction.Down)));
        allRooms.Add(new Room(new Door(Direction.Right), new Door(Direction.Left)));
        allRooms.Add(new Room(new Door(Direction.Down), new Door(Direction.Left)));

        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Right), new Door(Direction.Down)));
        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Right), new Door(Direction.Left)));
        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Down), new Door(Direction.Left)));
        allRooms.Add(new Room(new Door(Direction.Right), new Door(Direction.Down), new Door(Direction.Left)));

        allRooms.Add(new Room(new Door(Direction.Up), new Door(Direction.Right), new Door(Direction.Down), new Door(Direction.Left)));

        deadEndRooms = allRooms.Where(r => r.NumberOfDoors == 1).ToList();
        openRooms = allRooms.Where(r => r.NumberOfDoors > 1).ToList();
    }

    /// <summary>
    /// The function that triggers the creation of the entire level
    /// Sets up initial room then calls CreateMainPath to work out the main path through the level, Complete rooms to
    /// add rooms on to any doors without rooms connected to (challange rooms) and then build rooms to start turning the data
    /// in each room into a room in the scene
    /// </summary>
    /// <param name="minRoomsOnPath"></param>
    /// <param name="maxRoomsOnPath"></param>
    /// <param name="maxlengthOfOtherRoutes"></param>
    /// <returns></returns>
    public static Room CreateLevel(int minRoomsOnPath, int maxRoomsOnPath, int maxlengthOfOtherRoutes) {
        if (allRooms == null) {
            CreateAllRooms();
        }

        int pathLength = Random.Range(minRoomsOnPath, maxRoomsOnPath + 1);
        Room startingRoom = new Room(deadEndRooms[Random.Range(0, deadEndRooms.Count)]);
        Vector2 currentPosition = new Vector2(0f, 0f);
        startingRoom.position = currentPosition;
        startingRoom.roomType = RoomType.Starting;
        CreateMainPath(pathLength, startingRoom, currentPosition);
        CompleteRooms(startingRoom, maxlengthOfOtherRoutes);
        BuildRooms(startingRoom);
        RoomLayout.FinalizeLevel(pathLength);
        return startingRoom;

    }

    /// <summary>
    /// Creates a room layout for each room, which will then be made up from room piece prefabs
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <param name="visited"></param>
    private static void BuildRooms(Room currentRoom, List<Room> visited = null) {
        if (visited == null) {
            visited = new List<Room>();
        }
        if (currentRoom.roomType == RoomType.Boss) {
            RoomLayout.CreateBossRoom(currentRoom.doors, currentRoom.mapGO);
        } else {
            RoomLayout.GetRandomRoomLayout(currentRoom.doors, currentRoom.roomType, currentRoom.mapGO);
        }
        visited.Add(currentRoom);
        foreach (Door door in currentRoom.doors) {
            if (!visited.Contains(door.Room)) {
                if(door.Room == null) {
                    Debug.LogError("next room null");
                }
                BuildRooms(door.Room, visited);
            }
        }

    }

    /// <summary>
    /// Creates the main path through the level
    /// </summary>
    /// <param name="pathLength"></param>
    /// <param name="startingRoom"></param>
    /// <param name="currentPosition"></param>
    private static void CreateMainPath(int pathLength, Room startingRoom, Vector2 currentPosition) {
        positions = new List<Vector2>();
        Door door;
        int numberOfRoomsOnPath = 1;
        Room currentRoom = startingRoom;
        Direction lastDirection = Direction.Count;
        while (numberOfRoomsOnPath < pathLength) {
            positions.Add(currentRoom.position);
            currentRoom.IsOnMainPath = true;
            door = currentRoom.GetRandomDoor(lastDirection.GetOppositeDirection());
            foreach (Door exit in currentRoom.doors) {
                if (exit.Direction != lastDirection.GetOppositeDirection() && exit.Direction != door.Direction) {
                    positions.Add(exit.Direction.GetNextPosition(currentRoom.position));
                }
            }
            lastDirection = door.Direction;
            Room nextRoom = GetRandomRoom(door.Direction.GetOppositeDirection(), openRooms, currentRoom.position);
            nextRoom.IsOnMainPath = true;
            nextRoom.roomType = RoomType.MainPath;
            door.Room = nextRoom;
            GetDoorInDirection(nextRoom, door.Direction.GetOppositeDirection()).Room = currentRoom;
            currentRoom.mapGO = MapCreator.PlaceRoom(currentRoom);
            currentRoom = nextRoom;
            numberOfRoomsOnPath++;
        }
        positions.Add(currentRoom.position);
        currentRoom.IsOnMainPath = true;
        door = currentRoom.GetRandomDoor(lastDirection.GetOppositeDirection());
        currentRoom.mapGO = MapCreator.PlaceRoom(currentRoom);
        Room bossRoom = new Room(deadEndRooms.First(room => room.ContainsDoorDirection(door.Direction.GetOppositeDirection())));
        bossRoom.roomType = RoomType.Boss;
        bossRoom.position = door.Direction.GetNextPosition(currentRoom.position);
        positions.Add(bossRoom.position);
        bossRoom.IsOnMainPath = true;
        bossRoom.mapGO = MapCreator.PlaceRoom(bossRoom);
        door.Room = bossRoom;
        bossRoom.doors[0].Room = currentRoom;

    }

    /// <summary>
    /// Adds Challange rooms on to any doors that dont have a room connected
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <param name="maxDepth"></param>
    /// <param name="currentDepth"></param>
    /// <param name="visited"></param>
    private static void CompleteRooms(Room currentRoom, int maxDepth, int currentDepth = 0, List<Vector2> visited = null) {
        if(visited == null) {
            visited = new List<Vector2>();
        }
        visited.Add(currentRoom.position);
        foreach (Door door in currentRoom.doors) {
            if(visited.Contains(door.Direction.GetNextPosition(currentRoom.position))) {
                continue;
            }
            if(door.Room == null) {
                if (currentDepth + 1 >= maxDepth) {
                    door.Room = new Room(deadEndRooms.First(room => room.ContainsDoorDirection(door.Direction.GetOppositeDirection())));
                    door.Room.position = door.Direction.GetNextPosition(currentRoom.position);
                    door.Room.roomType = RoomType.Challange;
                    door.Room.mapGO = MapCreator.PlaceRoom(door.Room);
                } else {
                    door.Room = GetRandomRoom(door.Direction.GetOppositeDirection(), allRooms, currentRoom.position);
                    door.Room.position = door.Direction.GetNextPosition(currentRoom.position);
                    door.Room.roomType = RoomType.Challange;
                    door.Room.mapGO = MapCreator.PlaceRoom(door.Room);
                }
                Door connectedDoor = GetDoorInDirection(door.Room, door.Direction.GetOppositeDirection());
                connectedDoor.Room = currentRoom;
                
            }
            int depth = (door.Room.IsOnMainPath) ? 0 : currentDepth + 1;
            CompleteRooms(door.Room, maxDepth, depth, visited);

        }
    }

    /// <summary>
    /// Whether a position is valid or already contains a room
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private static bool IsValidPosition(Vector2 position) {
        return !positions.Contains(position);
    }

    /// <summary>
    /// Picks a random room to attach to a door
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="rooms"></param>
    /// <param name="sourcePosition"></param>
    /// <returns></returns>
    public static Room GetRandomRoom(Direction direction, List<Room> rooms, Vector2 sourcePosition) {
        List<Room> viableRooms = rooms.Where(room => room.ContainsDoorDirection(direction)).ToList();
        Room room;
        int count = 0;
        do {
            count++;
            room = new Room(viableRooms[Random.Range(0, viableRooms.Count)]);
            room.position = direction.GetOppositeDirection().GetNextPosition(sourcePosition);
        } while (count < 1000 && !ValidateRoomPositions(room, sourcePosition));
        if (count >= 1000) {
            Debug.LogError("NO VALID ROOM");
        }
        return room;
    }

    /// <summary>
    /// Gets the door from a room in a specific direction
    /// </summary>
    /// <param name="room"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static Door GetDoorInDirection(Room room, Direction direction) {
        if(room == null) {
            Debug.LogError("Room null");
        }
        if(room.doors == null) {
            Debug.LogError("Doors null");
        }
        foreach(Door door in room.doors) {
            if(door.Direction == direction) {
                return door;
            }
        }
        return null;
    }

    /// <summary>
    /// Ensures all room positions are valid
    /// </summary>
    /// <param name="room"></param>
    /// <param name="sourcePosition"></param>
    /// <returns></returns>
    private static bool ValidateRoomPositions(Room room, Vector2 sourcePosition) {
        foreach(Door door in room.doors) {
            if(door.Direction.GetNextPosition(room.position) == sourcePosition) {
                continue;
            }
            if (!IsValidPosition(door.Direction.GetNextPosition(room.position))) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// private constructor that takes one or more doors and creates an appropriate room
    /// </summary>
    /// <param name="doorsTocreate"></param>
    private Room(params Door[] doorsTocreate) {
        roomType = RoomType.MainPath;
        doors = new List<Door>(doorsTocreate.Length);
        foreach(Door door in doorsTocreate) {
            doors.Add(new Door(door.Direction));
        }
    }

    /// <summary>
    /// Constructor that copies a room
    /// </summary>
    /// <param name="room"></param>
    private Room(Room room) {
        roomType = RoomType.MainPath;
        doors = new List<Door>(room.doors.Count);
        foreach(Door door in room.doors) {
            doors.Add(new Door(door.Direction));
        }
    }

    /// <summary>
    /// Whether the room contains a door in a specific direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool ContainsDoorDirection(Direction direction) {
        foreach(Door door in doors) {
            if(door.Direction == direction) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Selects a random door from the doors in the room
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Door GetRandomDoor(Direction direction = Direction.Count) {
        if(doors.Count == 1) {
            return doors[0];
        }
        Door door;
        int safety = 0;
        do {
            door = doors[Random.Range(0, NumberOfDoors)];
            safety++;
        } while (door.Direction == direction && safety < 100000);
        if(safety >= 100000) {
            Debug.LogError("Safety exceeded");
        }
        return door;
    }

    /// <summary>
    /// Converts the door to a string made up of the directions of it's doors
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
        string toString = "";
        if (ContainsDoorDirection(Direction.Up)) {
            toString += "U";
        }
        if (ContainsDoorDirection(Direction.Right)) {
            toString += "R";
        }
        if (ContainsDoorDirection(Direction.Down)) {
            toString += "D";
        }
        if (ContainsDoorDirection(Direction.Left)) {
            toString += "L";
        }

        return toString;
    }

}
