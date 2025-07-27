public class Cell { 

  private int row = 0;
  private int col = 0;
  private Group neighbors;

  private byte value = 0;
  private byte[] candidate = new byte[10];
  private int count = 0;


  public String getName() {
    return "r" + (row+1) + "c" + (col+1);
  }

  public String getCandidates() {
    String s = "";
    if (value == 0) {
      for (int i=0; i < count; i++) s += "" + getCandidate(i);
    }
    return s;
  }

  public String toString() {
    String s = "[" + getName();
    if (value == 0) {
      s += "(" + getCandidates() + ")]";
    } else {
      s += "[" + value + "]]";
    }
    return s;
  }

  public Cell(int row, int col, Group neighbors) {
    this.row = row;
    this.col = col;
    this.neighbors = neighbors;

    reset();
  }

  public void reset() {
    value = 0;
    for (byte val=1; val < 10; val++) candidate[val] = 1;
    count = 9;
  }

  public Group getNeighbors() {
    return neighbors;
  }

  public byte getValue() {
    return value;
  }
  public void setValue(byte val) {
    value = val;
  }
  public void clearValue() {
    value = 0;
  }

  public int getRow() {
    return row;
  }
  public int getCol() {
    return col;
  }

  public boolean equals(Cell cell) {
    return cell != null && cell.row == row && cell.col == col;
  }


  /* candidates */

  public boolean addCandidate(byte val) {
    if (++candidate[val] == 1) count++;

    return value == 0 && count == 1;
  }

  public boolean removeCandidate(byte val) {
    if (--candidate[val] == 0) count--;

    return value == 0 && count == 0;
  }

  public boolean presentCandidate(byte val) {
    return value == 0 && candidate[val] == 1;
  }

  public int numberOfCandidates() {
    if (value != 0) return 0; else return count;
  }

  public byte getCandidate(int idx) {
    if (value != 0) return 0;
    int i = 0;
    for (byte val=1; val < 10 && i < count; val++) {
      if (candidate[val] == 1) {
        if (i++ == idx) return val;
      }
    }
    return 0;
  }

  public int getAllCandidates(byte cand[]) {
    if (value != 0) return 0;
    int i = 0;
    for (byte val=1; val < 10 && i < count; val++) {
      if (candidate[val] == 1) cand[i++] = val;
    }
    return count;
  }

  public byte[] getAllCandidates() {
    if (value != 0) return null;
    byte[] cand = new byte[count];
    getAllCandidates(cand);
    return cand;
  }

}

