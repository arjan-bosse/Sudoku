public class Group { 

  private Cell[] cells;
  private int size = 0;
  private String name = null;

  public Group(int maxSize, String name) {
    cells = new Cell[maxSize];
    this.name = name;
  }

  public void addCell(Cell cell) {
    if (cell == null) return;
    if (size == cells.length) return;

    for (int i=0; i < size; i++) {
      if (cells[i] == cell) return;
    }
    cells[size++] = cell;
  }

  public int getSize() {
    return size;
  }

  public Cell getCell(int idx) {
    if (idx < 0 || idx >= size) return null;
    else return cells[idx];
  }

  public String getName() {
    return name;
  }

  public String toString() {
    String s =  "[" + name;
    for (int i=0; i < size; i++) s += "," + cells[i];
    return s + "]";
  }
}