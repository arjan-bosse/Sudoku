public class Board { 

  private int filled = 0;
  private Cell[][] grid = new Cell[9][9];

  private Group   g_all = new Group(81, "grid");
  private Group[] g_row = new Group[9];
  private Group[] g_col = new Group[9];
  private Group[] g_box = new Group[9];

  public Board() {

    for (int i=0; i < 9; i++) {
      g_row[i] = new Group(9, "r" + (i+1));
      g_col[i] = new Group(9, "c" + (i+1));
      g_box[i] = new Group(9, "b" + (i+1));
    }

    for (int r=0; r < 9; r++) {
      for (int c=0; c < 9; c++) {
        Group nb = new Group((9-1)+2*(9-3), "neighbors r" + (r+1) + "c" + (c+1));
        Cell cell = new Cell(r, c, nb);

        grid[r][c] = cell;
        g_all.addCell(cell);
        g_row[r].addCell(cell);
        g_col[c].addCell(cell);
        g_box[ rowcol2box(r, c) ].addCell(cell);
      }
    }

    for (int r=0; r < 9; r++) {
      for (int c=0; c < 9; c++) {
        Cell cell = getCell(r, c);
        Group nb = cell.getNeighbors();
        Group excl = new Group(1, "");
        excl.addCell(cell);
        Group tmp = new Group(2*(9-1), "");
        merge(getRow(r), getCol(c), excl, tmp);
        merge(tmp, getBox(r, c), excl, nb);
      }
    }
  }

  public boolean solved() {
    return filled == 81;
  }

  public boolean impossible() {
    return filled < 17;
  }

  public int rowcol2box(int row, int col) {
    return 3*(row/3) + col/3;
  }

  public Cell presentValue(byte value, Group g) {
    for (int i=0; i < g.getSize(); i++) {
      Cell cell = g.getCell(i);
      if (cell.getValue() == value) return cell;
    }
    return null;
  }

  public Cell presentCandidate(byte value, Group g) {
    for (int i=0; i < g.getSize(); i++) {
      Cell cell = g.getCell(i);
      if (cell.presentCandidate(value)) return cell;
    }
    return null;
  }

  public Group candidateCells(byte value, Group g) {
    Group cand = new Group(9, g.getName());
    for (int i=0; i < g.getSize(); i++) {
      Cell cell = g.getCell(i);
      if (cell.presentCandidate(value)) cand.addCell(cell);
    }
    if (cand.getSize() > 0) return cand;
    return null;
  }

  public boolean present(Cell c, Group g) {
    for (int i=0; i < g.getSize(); i++) {
      if (g.getCell(i) == c) return true;
    }
    return false;
  }

  public void intersect(Group g1, Group g2, Group target) {
    for (int i=0; i < g1.getSize(); i++) {
      Cell cell = g1.getCell(i);
      if (present(cell, g2)) { 
        target.addCell(cell);
      }
    }
  }

  public void merge(Group g1, Group g2, Group exclude, Group target) {

    for (int i=0; i < g1.getSize(); i++) {
      Cell cell = g1.getCell(i);
      if (exclude != null) {
        if (present(cell, exclude)) continue;
      }
      target.addCell(cell);
    }

    if (g2 != null) {
      for (int i=0; i < g2.getSize(); i++) {
        Cell cell = g2.getCell(i);
        if (present(cell, g1)) continue;

        if (exclude != null) {
          if (present(cell, exclude)) continue;
        }
        target.addCell(cell);
      }
    }
  }

  public Group complementarySubset(Group domain, Group subset) {
    Group complement = new Group(domain.getSize() - subset.getSize(), "");
    merge(domain, null, subset, complement);
    return complement;
  }

  public Group getAll() {
    return g_all;
  }


  public Group getRow(int row) {
    return g_row[row];
  }
  public Group getCol(int col) {
    return g_col[col];
  }
  public Group getBox(int box) {
    return g_box[box];
  }
  public Group getBox(int row, int col) {
    return g_box[rowcol2box(row, col)];
  }


  public Group getRow(Cell cell) {
    return getRow(cell.getRow());
  }
  public Group getCol(Cell cell) {
    return getCol(cell.getCol());
  }
  public Group getBox(Cell cell) {
    return getBox(cell.getRow(), cell.getCol());
  }


  public Cell getCell(int row, int col) {
    return grid[row][col];
  }


  public void excludeCandidate(Group g, byte value) {
    for (int i=0; i < g.getSize(); i++) {
      g.getCell(i).removeCandidate(value);
    }
  }

  public void includeCandidate(Group g, byte value) {
    for (int i=0; i < g.getSize(); i++) {
      g.getCell(i).addCandidate(value);
    }
  }

  public void excludeCandidates(Group g, byte[] values) {
    for (int i=0; i < values.length; i++) {
      excludeCandidate(g, values[i]);
    }
  }

  public void includeCandidates(Group g, byte values[]) {
    for (int i=0; i < values.length; i++) {
      includeCandidate(g, values[i]);
    }
  }

  public byte[] complementaryValues(byte[] values) {
    byte[] complement = new byte[9-values.length];
    boolean map[] = new boolean[9+1];
    for (int i=0; i < values.length; i++) {
      map[values[i]] = true;
    }
    int idx = 0;
    for (byte v=1; v < 9+1; v++) {
      if (!map[v]) complement[idx++] = v;
    }
    return complement;
  }

  public void move(Cell cell, byte value) {
    excludeCandidate(cell.getNeighbors(), value);
    cell.setValue(value);
    filled++;
  }

  public void undo(Cell cell, byte value) {
    filled--;
    cell.clearValue();
    includeCandidate(cell.getNeighbors(), value);
  }

  public Cell getCellWithFewestCandidates(Group g) {
    int min = 9+1;
    Cell candidate = null;

    for (int i=0; i < g.getSize(); i++) {
      Cell cell = g.getCell(i);
      if (cell.getValue() == 0 && cell.numberOfCandidates() < min) {
        candidate = cell;  
        min = cell.numberOfCandidates();
      }
    }
    return candidate;
  }

  public byte[] mergeCandidates(byte[] c1, byte[] c2) {
    boolean[] map = new boolean[9+1];
    int count = 0;

    for (int i=0; i < c1.length; i++) {
      if (!map[c1[i]]) { map[c1[i]] = true; count++; }
    }
    for (int i=0; i < c2.length; i++) {
      if (!map[c2[i]]) { map[c2[i]] = true; count++; }
    }
    byte[] merged = new byte[count];
    int idx = 0;
    for (byte v=1; v < 9+1 && idx < count; v++) {
      if (map[v]) merged[idx++] = v;
    }
    return merged;
  }

  /*
  public void export(byte[][] grid) {
    for (int r=0; r < 9; r++) {
      for (int c=0; c < 9; c++) {
        grid[r][c] = getCell(r, c).getValue();
      }
    }
  }
  */

  public void setup(byte[][] grid, String[][] cand) {
    filled = 0;

    Group all = getAll();
    for (int i=0; i < all.getSize(); i++) {
       all.getCell(i).reset();
    }

    for (int r=0; r < 9; r++) {
      for (int c=0; c < 9; c++) {
        Cell cell = getCell(r, c);
        if (grid[r][c] == 0) {
          cell.clearValue();
          if (cand[r][c] != null) {
            for (byte v=1; v < 9+1; v++) {
              if (!cand[r][c].contains("" + v)) cell.removeCandidate(v);
            }
          }
        } else {
          move(cell, grid[r][c]);
        }
      }
    }
  }

}