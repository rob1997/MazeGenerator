# Procedural Maze Generator

A procedural Maze Generator the uses the Hunt and Kill Algorithm
- Can generate upto a 45 * 45 maze (more than that casues a stack overflow error because the algorithm uses recurssion)

### `MazeGenerator.cs`
- `_cells[x, z]` : is a 2 dimensional maxtrix representing all the cells of the maze
- `size` : is an Integer Vecotr2 (`IntVector2`) representing the x and z size of the maze
- `cellUnitSize` : is how wide a single cell is (a cell must be a square)
- `cellPrefab` : is a prefab of a singular cell with all 4 walls

### `Cell.cs`
- `coordinate` : an Integer Vector2 that is the location of the cell in cell units
- `walls` : a list of the `Wall` struct that contains the `Direction` enum a wall is facing and transform of a wall

look at the Demo scene
![demo maze](https://user-images.githubusercontent.com/36323674/106953929-143e5b00-6744-11eb-996c-261ab687d22f.png)
