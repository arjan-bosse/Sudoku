import java.io.*;

// Sudoku (Java)
//
// Version: 0.1 (original)
// Date   : 2005-09-12
// Author : Arjan Bosse
//
// Version: 0.2 (fix)
// Date   : 2018-02-12
// Author : Arjan Bosse

public class Sudoku {

private Board b = new Board();
private String[] history = new String[81*(9+1)];
private int depth = 0;

private int naked_single = 0;
private int hidden_single = 0;
private int naked_pair = 0;
private int hidden_pair = 0;
private int naked_triple = 0;
private int hidden_triple = 0;
private int naked_quad = 0;
private int hidden_quad = 0;
private int locked_candidate = 0;
private int x_wing = 0;
private int swordfish = 0;
private int yellyfish = 0;
private int xy_wing = 0;
private int guess = 0;

private static boolean show_history;
private static boolean show_input;
private static boolean show_output;
private static boolean show_result;
private static boolean show_summary;
private static boolean trial_and_error;

private enum Results { NOTHING, SOLUTION, MULTIPLE, IMPOSSIBLE, UNSOLVABLE, UNFINISHED };
private Results result = Results.NOTHING;

// INPUT

private static void readInput(byte[][] grid, String[][] cand) throws IOException {
  InputStreamReader tempReader = new InputStreamReader(System.in);
  BufferedReader reader = new BufferedReader(tempReader);

  for (int x=0; ; ) {
    String line = reader.readLine();
    char[] chars = line.toCharArray();
    boolean rc = false;

    for (int i=0; i < chars.length; i++) {
      if (chars[i] == '.' || chars[i] == '0' || chars[i] == 'O' || chars[i] == 'o') {
        if (!rc) {
          grid[x/9][x%9] = 0;
          if (++x == 81) return;
        }
      } else if (Character.isDigit(chars[i])) {
        if (rc) {
          cand[x/9][x%9] += chars[i];
        } else {
          grid[x/9][x%9] = (new Integer(Character.digit(chars[i], 10))).byteValue();
          if (++x == 81) return;
        }
      } else if (chars[i] == '(') {
        rc = true;
        cand[x/9][x%9] = "";
      } else if (chars[i] == ')') {
        rc = false;
        grid[x/9][x%9] = 0;
        if (++x == 81) return;
      }
    }
  }
}

private static void showInput(byte[][] grid, String[][] cand) {
  if (!show_input) return;
  for (int r=0; r < 9; r++) {
    if (r==3 || r==6) System.out.println("---+---+---");
    String line = "";
    for (int c=0; c < 9; c++) {
      if (c==3 || c==6) line += "|";
      if (grid[r][c] == 0) {
        line += (cand[r][c] == null ? '.' : '(' + cand[r][c] + ')');
      } else {
        line += grid[r][c];
      }
    }
    System.out.println(line);
  }
  System.out.println();
}

// OUTPUT

private void reportHistory() {
  if (!show_history) return;
  for (int i=0; i < depth; i++) {
    System.out.println(history[i]);
  }
  System.out.println();
}

private void reportSummary() {
  if (!show_summary) return;
  if (naked_single     > 0) System.out.println("naked single "     + naked_single);
  if (hidden_single    > 0) System.out.println("hidden single "    + hidden_single);
  if (naked_pair       > 0) System.out.println("naked pair "       + naked_pair);
  if (hidden_pair      > 0) System.out.println("hidden pair "      + hidden_pair);
  if (naked_triple     > 0) System.out.println("naked triple "     + naked_triple);
  if (hidden_triple    > 0) System.out.println("hidden triple "    + hidden_triple);
  if (naked_quad       > 0) System.out.println("naked quad "       + naked_quad);
  if (hidden_quad      > 0) System.out.println("hidden quad "      + hidden_quad);
  if (locked_candidate > 0) System.out.println("locked candidate " + locked_candidate);
  if (x_wing           > 0) System.out.println("x-wing "           + x_wing);
  if (swordfish        > 0) System.out.println("swordfish "        + swordfish);
  if (yellyfish        > 0) System.out.println("yellyfish "        + yellyfish);
  if (xy_wing          > 0) System.out.println("xy-wing "          + xy_wing);
  if (guess            > 0) System.out.println("guess "            + guess);
  System.out.println();
}

private void reportOutput() {
  if (!show_output) return;
  showCandidates();
}

private void report(Results res) {
  if (result == Results.SOLUTION && res == Results.SOLUTION) {
    result = Results.MULTIPLE;
  } else {
    reportHistory();
    reportSummary();
    reportOutput();
    result = res;
  }
}

private void showCandidates() {
  int min = 0;
  for (int r=0; r < 9; r++) {
    for (int c=0; c < 9; c++) {
      Cell cell = b.getCell(r, c);
      if (cell.numberOfCandidates() > min) {
        min = cell.numberOfCandidates();
      }
    }
  }
  int len;
  if (min == 0) len = 1; else len = min + 2;

  for (int r=0; r < 9; r++) {
    if (r==3 || r==6) {
      for (int i=0; i < 3*len; i++) System.out.print("-");
      System.out.print("+");
      for (int i=0; i < 3*len; i++) System.out.print("-");
      System.out.print("+");
      for (int i=0; i < 3*len; i++) System.out.print("-");
      System.out.println();
    }
    for (int c=0; c < 9; c++) {
      Cell cell = b.getCell(r, c);
      if (c==3 || c==6) System.out.print("|");
      if (cell.getValue() != 0) {
        System.out.print("" + cell.getValue());
        for (int i=1; i < len; i++) System.out.print(" ");
      } else {
        System.out.print("(" + cell.getCandidates() + ")");
        for (int i=cell.numberOfCandidates()+2; i < len; i++) System.out.print(" ");
      }
    }
    System.out.println();
  }
  System.out.println();
}

// NAKED SINGLE

private boolean solveNakedSingle(Cell cell) {
  if (cell.numberOfCandidates() == 1) {
    byte value = cell.getCandidate(0);

    String info = "naked single " + value + " in " + cell.getName();

    b.move(cell, value);
    naked_single++; solve(info); naked_single--;
    b.undo(cell, value);

    return true;
  }
  return false;
}

// HIDDEN SINGLE

private Cell getHiddenSingle(Group g, byte[] values) {
  int[] sum = new int[9+1];
  Cell[] idx = new Cell[9+1];

  for (int j=0; j < g.getSize(); j++) {
    Cell cell = g.getCell(j);
    if (cell.getValue() == 0) {
      byte[] cand = new byte[9];

      cell.getAllCandidates(cand);
      for (int i=0; i < cell.numberOfCandidates(); i++) {
        byte val = cand[i];
 
        sum[val]++;
        idx[val] = cell;
      }
    }
  }
  for (byte v=1; v < 9+1; v++) {
    if (sum[v] == 1) {
      values[0] = v;
      return idx[v];
    }
  }
  return null;
}

private boolean solvedHiddenSingle(Group g) {
  byte[] values = new byte[1];
  Cell cell = getHiddenSingle(g, values);
  if (cell != null) {

    String info = "hidden single " + values[0] + " in " + cell.getName();

    b.move(cell, values[0]);
    hidden_single++; solve(info); hidden_single--;
    b.undo(cell, values[0]);

    return true;
  }
  return false;
}

private boolean solveHiddenSingle() {
  for (int r=0; r < 9; r++) if (solvedHiddenSingle(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedHiddenSingle(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedHiddenSingle(b.getBox(x))) return true;
  return false;
}

// NAKED PAIR

private Group getNakedPair(Group g, byte[] cand) {

  for (int i=0; i < g.getSize() - 1; i++) {
  Cell c1 = g.getCell(i);
  if (c1.numberOfCandidates() == 2) {
  c1.getAllCandidates(cand);

  for (int j=i+1; j < g.getSize(); j++) {
  Cell c2 = g.getCell(j);
  if (c2.numberOfCandidates() == 2 &&
  c2.presentCandidate(cand[0]) &&
  c2.presentCandidate(cand[1]) ){

  /* pair found, now ensure that at least one candidate will be excluded elsewhere */
  for (int z=0; z < g.getSize(); z++) {
  Cell c9 = g.getCell(z);
  if (z != i &&
      z != j &&
      c9.numberOfCandidates() > 0 && (
      c9.presentCandidate(cand[0]) ||
      c9.presentCandidate(cand[1]) )){

      Group pair = new Group(2, "");
      pair.addCell(c1);
      pair.addCell(c2);
      return pair;
  }}}}}}
  return null;
}

private boolean solvedNakedPair(Group g) {
  byte[] values = new byte[2];
  Group pair = getNakedPair(g, values);
  if (pair != null) {

    String info = "naked pair " + values[0] + "," + values[1] + " in " + g.getName();

    b.excludeCandidates(b.complementarySubset(g, pair), values);
    naked_pair++; solve(info); naked_pair--;
    b.includeCandidates(b.complementarySubset(g, pair), values);

    return true;
  }
  return false;
}

private boolean solveNakedPair() {
  for (int r=0; r < 9; r++) if (solvedNakedPair(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedNakedPair(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedNakedPair(b.getBox(x))) return true;
  return false;
}

// HIDDEN PAIR

private Group getHiddenPair(Group g, byte[] values) {

  for (int i=0; i < g.getSize() - 1; i++) {
  Cell c1 = g.getCell(i);
  if (c1.numberOfCandidates() >= 2) {

  for (int j=i+1; j < g.getSize(); j++) {
  Cell c2 = g.getCell(j);
  if (c2.numberOfCandidates() >= 2) {

  // ensure that at least another candidate can be excluded from pair
  if (c1.numberOfCandidates() + c2.numberOfCandidates() == 4) continue;

  for (byte v=1; v < 9+1-1; v++) {
  if (c1.presentCandidate(v) && c2.presentCandidate(v)) {
  byte w=v;
  nxt_w: for (++w; w < 9+1; w++) {
  if (c1.presentCandidate(w) && c2.presentCandidate(w)) {

  for (int k=0; k < g.getSize(); k++) {
  if (k != i && k != j) {
  Cell cell = g.getCell(k);
  if (cell.presentCandidate(v) || cell.presentCandidate(w)) continue nxt_w;
  }}

  Group pair = new Group(2, "");
  pair.addCell(c1); pair.addCell(c2);
  values[0] = v; values[1] = w;
  return pair;                      
  }}}}}}}}
  return null;
}

private boolean solvedHiddenPair(Group g) {
  byte[] values = new byte[2];
  Group pair = getHiddenPair(g, values);
  if (pair != null) {

    String info = "hidden pair " + values[0] + "," + values[1] + " in " + g.getName();

    b.excludeCandidates(pair, b.complementaryValues(values));
    hidden_pair++; solve(info); hidden_pair--;
    b.includeCandidates(pair, b.complementaryValues(values));

    return true;
  }
  return false;
}

private boolean solveHiddenPair() {
  for (int r=0; r < 9; r++) if (solvedHiddenPair(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedHiddenPair(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedHiddenPair(b.getBox(x))) return true;
  return false;
}

// NAKED TRIPLE

private Group getNakedTriple(Group g, byte[] cand) {

  for (int i=0; i < g.getSize() - 2; i++) {
  Cell c1 = g.getCell(i);
  if (c1.numberOfCandidates() >= 2 && c1.numberOfCandidates() <= 3) {
  byte[] cand1 = c1.getAllCandidates();

  for (int j=i+1; j < g.getSize() - 1; j++) {
  Cell c2 = g.getCell(j);
  if (c2.numberOfCandidates() >= 2 && c2.numberOfCandidates() <= 3) {
  byte[] cand2 = b.mergeCandidates(cand1, c2.getAllCandidates());
  if (cand2.length <= 3) {

  for (int k=j+1; k < g.getSize(); k++) {
  Cell c3 = g.getCell(k);
  if (c3.numberOfCandidates() >= 2 && c3.numberOfCandidates() <= 3) {
  byte[] cand3 = b.mergeCandidates(cand2, c3.getAllCandidates());
  if (cand3.length == 3) {

  //System.out.println("possible naked triple " + c1 + c2 + c3);

  /* triple found, now ensure that at least one candidate will be excluded elsewhere */
  for (int z=0; z < g.getSize(); z++) {
  Cell c9 = g.getCell(z);
  if (z != i &&
      z != j &&
      z != k &&
      c9.numberOfCandidates() > 0) {

      if (b.mergeCandidates(cand3, c9.getAllCandidates()).length < cand3.length + c9.numberOfCandidates()) {
      for (int y=0; y < cand3.length; y++) cand[y] = cand3[y];

      Group triple = new Group(3, "");
      triple.addCell(c1);
      triple.addCell(c2);
      triple.addCell(c3);
      return triple;
  }}}}}}}}}}}
  return null;
}

private boolean solvedNakedTriple(Group g) {
  byte[] values = new byte[3];
  Group triple = getNakedTriple(g, values);
  if (triple != null) {

    String info = "naked triple " + values[0] + "," + values[1] + "," + values[2] + " in " + g.getName();

    b.excludeCandidates(b.complementarySubset(g, triple ), values);
    naked_triple++; solve(info); naked_triple--;
    b.includeCandidates(b.complementarySubset(g, triple ), values);

    return true;
  }
  return false;
}

private boolean solveNakedTriple() {
  for (int r=0; r < 9; r++) if (solvedNakedTriple(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedNakedTriple(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedNakedTriple(b.getBox(x))) return true;
  return false;
}

// HIDDEN TRIPLE

private Group getHiddenTriple(Group g, byte[] values) {

  int[] cnt = new int[9+1];
  Cell[][] map = new Cell[9+1][9];

  for (int i=0; i < g.getSize(); i++) {
    Cell cell = g.getCell(i);
    if (cell.numberOfCandidates() < 2) continue;
    for (byte v=1; v < 9+1; v++) {
      if (cell.presentCandidate(v)) {
        map[v][cnt[v]] = cell;
        cnt[v]++;
      }
    }
  }

  for (byte v=1; v < 9+1-2; v++) {
  if (cnt[v] < 2 || cnt[v] > 3) continue;
  Group g1 = new Group(cnt[v], "");
  for (int i=0; i < cnt[v]; i++) g1.addCell(map[v][i]);

  byte w=v; for (w++; w < 9+1-1; w++) {
  if (cnt[w] < 2 || cnt[w] > 3) continue;
  Group g2 = new Group(g1.getSize() + cnt[w], ""); b.merge(g1, null, null, g2);
  for (int i=0; i < cnt[w]; i++) g2.addCell(map[w][i]);
  if (g2.getSize() > 3) continue;

  byte x=w; for (x++; x < 9+1; x++) {
  if (cnt[x] < 2 || cnt[x] > 3) continue;
  Group g3 = new Group(g2.getSize() + cnt[x], ""); b.merge(g2, null, null, g3);
  nxt_i: for (int i=0; i < cnt[x]; i++) g3.addCell(map[x][i]);
  if (g3.getSize() != 3) continue;

  //System.out.println("possible hidden triple " + g3);

  // ensure that at least another candidate can be eliminated in the triple
  for (byte z=1; z < 9+1; z++) {
    if (z == v || z == w || z == x) continue;
    if (b.presentCandidate(z, g3) != null) {
      values[0] = v; values[1] = w; values[2] = x;
      return g3;
    }
  }

  }}}
  return null;
}

private boolean solvedHiddenTriple(Group g) {
  byte[] values = new byte[3];
  Group triple = getHiddenTriple(g, values);
  if (triple != null) {

    String info = "hidden triple " + values[0] + "," + values[1] + "," + values[2] + " in " + g.getName();

    b.excludeCandidates(triple , b.complementaryValues(values));
    hidden_triple++; solve(info); hidden_triple--;
    b.includeCandidates(triple , b.complementaryValues(values));

    return true;
  }
  return false;
}

private boolean solveHiddenTriple() {
  for (int r=0; r < 9; r++) if (solvedHiddenTriple(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedHiddenTriple(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedHiddenTriple(b.getBox(x))) return true;
  return false;
}

// NAKED QUAD

private Group getNakedQuad(Group g, byte[] cand) {

  for (int i=0; i < g.getSize() - 3; i++) {
  Cell c1 = g.getCell(i);
  if (c1.numberOfCandidates() >= 2 && c1.numberOfCandidates() <= 4) {
  byte[] cand1 = c1.getAllCandidates();

  for (int j=i+1; j < g.getSize() - 2; j++) {
  Cell c2 = g.getCell(j);
  if (c2.numberOfCandidates() >= 2 && c2.numberOfCandidates() <= 4) {
  byte[] cand2 = b.mergeCandidates(cand1, c2.getAllCandidates());
  if (cand2.length <= 4) {

  for (int k=j+1; k < g.getSize() - 1; k++) {
  Cell c3 = g.getCell(k);
  if (c3.numberOfCandidates() >= 2 && c3.numberOfCandidates() <= 4) {
  byte[] cand3 = b.mergeCandidates(cand2, c3.getAllCandidates());
  if (cand3.length <= 4) {

  for (int m=k+1; m < g.getSize(); m++) {
  Cell c4 = g.getCell(m);
  if (c4.numberOfCandidates() >= 2 && c4.numberOfCandidates() <= 4) {
  byte[] cand4 = b.mergeCandidates(cand3, c4.getAllCandidates());
  if (cand4.length == 4) {

  //System.out.println("possible naked quad " + c1 + c2 + c3 + c4);

  /* quad found, now ensure that at least one candidate will be excluded elsewhere */
  for (int z=0; z < g.getSize(); z++) {
  Cell c9 = g.getCell(z);
  if (z != i &&
      z != j &&
      z != k &&
      z != m &&
      c9.numberOfCandidates() > 0) {

      if (b.mergeCandidates(cand4, c9.getAllCandidates()).length < cand4.length + c9.numberOfCandidates()) {
      for (int y=0; y < cand4.length; y++) cand[y] = cand4[y];

      Group quad = new Group(4, "");
      quad.addCell(c1);
      quad.addCell(c2);
      quad.addCell(c3);
      quad.addCell(c4);
      return quad;
  }}}}}}}}}}}}}}
  return null;
}

private boolean solvedNakedQuad(Group g) {
  byte[] values = new byte[4];
  Group quad = getNakedQuad(g, values);
  if (quad != null) {

    String info = "naked quad " + values[0] + "," + values[1] + "," + values[2] + "," + values[3] + " in " + g.getName();
    //System.out.println(info + quad);

    b.excludeCandidates(b.complementarySubset(g, quad), values);
    naked_quad++; solve(info); naked_quad--;
    b.includeCandidates(b.complementarySubset(g, quad), values);

    return true;
  }
  return false;
}

private boolean solveNakedQuad() {
  for (int r=0; r < 9; r++) if (solvedNakedQuad(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedNakedQuad(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedNakedQuad(b.getBox(x))) return true;
  return false;
}

// HIDDEN QUAD

private Group getHiddenQuad(Group g, byte[] values) {

  int[] cnt = new int[9+1];
  Cell[][] map = new Cell[9+1][9];

  for (int i=0; i < g.getSize(); i++) {
    Cell cell = g.getCell(i);
    if (cell.numberOfCandidates() < 2) continue;
    for (byte v=1; v < 9+1; v++) {
      if (cell.presentCandidate(v)) {
        map[v][cnt[v]] = cell;
        cnt[v]++;
      }
    }
  }

  for (byte v=1; v < 9+1-3; v++) {
  if (cnt[v] < 2 || cnt[v] > 4) continue;
  Group g1 = new Group(cnt[v], "");
  for (int i=0; i < cnt[v]; i++) g1.addCell(map[v][i]);

  byte w=v; for (w++; w < 9+1-2; w++) {
  if (cnt[w] < 2 || cnt[w] > 4) continue;
  Group g2 = new Group(g1.getSize() + cnt[w], ""); b.merge(g1, null, null, g2);
  for (int i=0; i < cnt[w]; i++) g2.addCell(map[w][i]);
  if (g2.getSize() > 4) continue;

  byte x=w; for (x++; x < 9+1-1; x++) {
  if (cnt[x] < 2 || cnt[x] > 4) continue;
  Group g3 = new Group(g2.getSize() + cnt[x], ""); b.merge(g2, null, null, g3);
  for (int i=0; i < cnt[x]; i++) g3.addCell(map[x][i]);
  if (g3.getSize() > 4) continue;

  byte y=x; for (y++; y < 9+1; y++) {
  if (cnt[y] < 2 || cnt[y] > 4) continue;
  Group g4 = new Group(g3.getSize() + cnt[y], ""); b.merge(g3, null, null, g4);
  nxt_i: for (int i=0; i < cnt[y]; i++) g4.addCell(map[y][i]);
  if (g4.getSize() != 4) continue;

  //System.out.println("possible hidden quad " + g4);

  // ensure that at least another candidate can be eliminated in the triple
  for (byte z=1; z < 9+1; z++) {
    if (z == v || z == w || z == x || z == y) continue;
    if (b.presentCandidate(z, g4) != null) {
      values[0] = v; values[1] = w; values[2] = x; values[3] = y;
      return g4;
    }
  }

  }}}}
  return null;
}

private boolean solvedHiddenQuad(Group g) {
  byte[] values = new byte[4];
  Group quad = getHiddenQuad(g, values);
  if (quad != null) {

    String info = "hidden quad " + values[0] + "," + values[1] + "," + values[2] + "," + values[3] + " in " + g.getName();

    b.excludeCandidates(quad, b.complementaryValues(values));
    hidden_quad++; solve(info); hidden_quad--;
    b.includeCandidates(quad, b.complementaryValues(values));

    return true;
  }
  return false;
}

private boolean solveHiddenQuad() {
  for (int r=0; r < 9; r++) if (solvedHiddenQuad(b.getRow(r))) return true;
  for (int c=0; c < 9; c++) if (solvedHiddenQuad(b.getCol(c))) return true;
  for (int x=0; x < 9; x++) if (solvedHiddenQuad(b.getBox(x))) return true;
  return false;
}

// LOCKED CANDIDATE

private Group getLockedCandidate(Group s, Group t, byte[] values) {
  Group  g = new Group(  3, ""); b.intersect(s, t, g );
  Group sd = new Group(9-3, ""); b.merge(s, null, g, sd);
  Group td = new Group(9-3, ""); b.merge(t, null, g, td);

  for (byte v=1; v < 10; v++) {
    if (b.presentCandidate(v, g) != null) {
      if (b.presentCandidate(v, sd) != null && b.presentCandidate(v, td) == null) {
        values[0] = v;
        return sd;
      }
      if (b.presentCandidate(v, sd) == null && b.presentCandidate(v, td) != null) {
        values[0] = v;
        return td;
      }
    }
  }
  return null;
}

private boolean solvedLockedCandidate(Group g1, Group g2) {
  byte[] values = new byte[1];
  Group excludetarget = getLockedCandidate(g1, g2, values);
  if (excludetarget!= null) {

    String info = "locked candidate " + values[0] + " in " + g1.getName() + g2.getName();

    //showCandidates();

    b.excludeCandidate(excludetarget, values[0]);
    locked_candidate++; solve(info); locked_candidate--;
    b.includeCandidate(excludetarget, values[0]);

    return true;
  }
  return false;
}

private boolean solveLockedCandidate() {
  for (int x=0; x < 9; x++) {
    Group box = b.getBox(x);

    int r0 = box.getCell(0).getRow();
    int c0 = box.getCell(0).getCol();

    for (int r=r0; r < r0+3; r++) if (solvedLockedCandidate(box, b.getRow(r))) return true;
    for (int c=c0; c < c0+3; c++) if (solvedLockedCandidate(box, b.getCol(c))) return true;
  }
  return false;
}

// X-WING

private boolean solvedXWing(byte value, Group g1, Group g2, Group g3, Group g4) {

  // ensure at least one candidate will be excluded
  if (g3.getSize() == 2 && g4.getSize() == 2) return false;

  Group tmp = new Group(2+2, ""); b.merge(g1, g2, null, tmp);
  Group cnd = new Group(9+9-2-2, ""); b.merge(g3, g4, tmp, cnd);

  String info = "x-wing " + value + " in " + g1.getName() + g2.getName();
        
  b.excludeCandidate(cnd, value);
  x_wing++; solve(info); x_wing--;
  b.includeCandidate(cnd, value);

  return true;
}

private boolean solveXWing() {

  for (byte v=1; v < 9+1; v++) {

    for (int r1=0; r1 < 9-1; r1++) {
      Group g1 = b.candidateCells(v, b.getRow(r1));
      if (g1 == null || g1.getSize() != 2) continue;

      int c1 = g1.getCell(0).getCol();
      int c2 = g1.getCell(1).getCol();

      for (int r2 = r1+1; r2 < 9; r2++) {
        if (b.getBox(r1, c1) == b.getBox(r2, c2)) continue; // no 4 cells in 1 box

        Group g2 = b.candidateCells(v, b.getRow(r2));
        if (g2 == null || g2.getSize() != 2) continue;

        if (g2.getCell(0).getCol() != c1 || g2.getCell(1).getCol() != c2) continue;

        Group g3 = b.candidateCells(v, b.getCol(c1));
        Group g4 = b.candidateCells(v, b.getCol(c2));

        if (solvedXWing(v, g1, g2, g3, g4)) return true;
      }
    }

    for (int c1=0; c1 < 9-1; c1++) {
      Group g1 = b.candidateCells(v, b.getCol(c1));
      if (g1 == null || g1.getSize() != 2) continue;

      int r1 = g1.getCell(0).getRow();
      int r2 = g1.getCell(1).getRow();

      for (int c2 = c1+1; c2 < 9; c2++) {
        if (b.getBox(r1, c1) == b.getBox(r2, c2)) continue;

        Group g2 = b.candidateCells(v, b.getCol(c2));
        if (g2 == null || g2.getSize() != 2) continue;

        if (g2.getCell(0).getRow() != r1 || g2.getCell(1).getRow() != r2) continue;

        Group g3 = b.candidateCells(v, b.getRow(r1));
        Group g4 = b.candidateCells(v, b.getRow(r2));

        if (solvedXWing(v, g1, g2, g3, g4)) return true;
      }
    }
  }

  return false;
}


// SWORDFISH

private int[] merge(int[] a, int[] b) {
  boolean[] map = new boolean[9];
  int cnt = 0;
  for (int i=0; i < a.length; i++) { map[a[i]] = true; cnt++; }
  for (int i=0; i < b.length; i++) if (!map[b[i]]) { map[b[i]] = true; cnt++; }
  int[] c = new int[cnt];
  int idx = 0;
  for (int i=0; i < 9 && idx < cnt; i++) if (map[i]) c[idx++] = i; 
  return c;
}

private int[] getRows(Group g) {
  int[] rows = new int[g.getSize()];
  for (int i=0; i < g.getSize(); i++) rows[i] = g.getCell(i).getRow();
  return rows;
}

private int[] getColumns(Group g) {
  int[] cols = new int[g.getSize()];
  for (int i=0; i < g.getSize(); i++) cols[i] = g.getCell(i).getCol();
  return cols;
}

private boolean solvedSwordfish(byte value, Group g1, Group g2, Group g3, Group g4, Group g5, Group g6) {

  // ensure at least one candidate will be excluded
  Group tmp12 = new Group(3+3, ""); b.merge(g1, g2, null, tmp12);
  Group tmp123 = new Group(3+3+3, ""); b.merge(tmp12, g3, null, tmp123);
  Group tmp45 = new Group(9+9, ""); b.merge(g4, g5, null, tmp45);
  Group tmp456 = new Group(9+9+9, ""); b.merge(tmp45, g6, null, tmp456);
  Group cnd = new Group(9+9+9, ""); b.merge(tmp456, null, tmp123, cnd);

  if (cnd.getSize() > 0) {

    String info = "swordfish " + value + " in " + g1.getName() + g2.getName() + g3.getName();
        
    b.excludeCandidate(cnd, value);
    swordfish++; solve(info); swordfish--;
    b.includeCandidate(cnd, value);

    return true;
  }

  return false;
}

private boolean solveSwordfish() {

  for (byte v=1; v < 9+1; v++) {

    for (int r1=0; r1 < 9-2; r1++) {
      Group g1 = b.candidateCells(v, b.getRow(r1));
      if (g1 == null || g1.getSize() > 3) continue;
      int[] cols1 = getColumns(g1);

      for (int r2 = r1+1; r2 < 9-1; r2++) {
      Group g2 = b.candidateCells(v, b.getRow(r2));
      if (g2 == null || g2.getSize() > 3) continue;
      int[] cols2 = merge(cols1, getColumns(g2));
      if (cols2.length > 3) continue;

      for (int r3 = r2+1; r3 < 9; r3++) {
      Group g3 = b.candidateCells(v, b.getRow(r3));
      if (g3 == null || g3.getSize() > 3) continue;
      int[] cols3 = merge(cols2, getColumns(g3));
      if (cols3.length != 3) continue;

      //System.out.println("possible swordfish " + v + " in " + "r"+(r1+1) + "r"+(r2+1) + "r"+(r3+1));

      Group g4 = b.candidateCells(v, b.getCol(cols3[0]));
      Group g5 = b.candidateCells(v, b.getCol(cols3[1]));
      Group g6 = b.candidateCells(v, b.getCol(cols3[2]));

      if (solvedSwordfish(v, g1, g2, g3, g4, g5, g6)) return true;
    }}}

    for (int c1=0; c1 < 9-2; c1++) {
      Group g1 = b.candidateCells(v, b.getCol(c1));
      if (g1 == null || g1.getSize() > 3) continue;
      int[] rows1 = getRows(g1);

      for (int c2 = c1+1; c2 < 9-1; c2++) {
      Group g2 = b.candidateCells(v, b.getCol(c2));
      if (g2 == null || g2.getSize() > 3) continue;
      int[] rows2 = merge(rows1, getRows(g2));
      if (rows2.length > 3) continue;

      for (int c3 = c2+1; c3 < 9; c3++) {
      Group g3 = b.candidateCells(v, b.getCol(c3));
      if (g3 == null || g3.getSize() > 3) continue;
      int[] rows3 = merge(rows2, getRows(g3));
      if (rows3.length != 3) continue;

      //System.out.println("possible swordfish " + v + " in " + "c"+(c1+1) + "c"+(c2+1) + "c"+(c3+1));

      Group g4 = b.candidateCells(v, b.getRow(rows3[0]));
      Group g5 = b.candidateCells(v, b.getRow(rows3[1]));
      Group g6 = b.candidateCells(v, b.getRow(rows3[2]));

      if (solvedSwordfish(v, g1, g2, g3, g4, g5, g6)) return true;
    }}}
  }
  return false;
}

// YELLYFISH

private boolean solvedYellyfish(byte value, Group g1, Group g2, Group g3, Group g4,
  Group g5, Group g6, Group g7, Group g8) {

  // ensure at least one candidate will be excluded
  Group tmp12 = new Group(4+4, ""); b.merge(g1, g2, null, tmp12);
  Group tmp123 = new Group(4+4+4, ""); b.merge(tmp12, g3, null, tmp123);
  Group tmp1234 = new Group(4+4+4+4, ""); b.merge(tmp123, g4, null, tmp1234);

  Group tmp56 = new Group(9+9, ""); b.merge(g5, g6, null, tmp56);
  Group tmp567 = new Group(9+9+9, ""); b.merge(tmp56, g7, null, tmp567);
  Group tmp5678 = new Group(9+9+9+9, ""); b.merge(tmp567, g8, null, tmp5678);

  Group cnd = new Group(9+9+9+9, ""); b.merge(tmp5678, null, tmp1234, cnd);

  if (cnd.getSize() > 0) {

    String info = "yellyfish " + value + " in " + g1.getName() + g2.getName() + g3.getName() + g4.getName();

    b.excludeCandidate(cnd, value);
    yellyfish++; solve(info); yellyfish--;
    b.includeCandidate(cnd, value);

    return true;
  }

  return false;
}

private boolean solveYellyfish() {

  for (byte v=1; v < 9+1; v++) {

    for (int r1=0; r1 < 9-3; r1++) {
      Group g1 = b.candidateCells(v, b.getRow(r1));
      if (g1 == null || g1.getSize() > 4) continue;
      int[] cols1 = getColumns(g1);

      for (int r2 = r1+1; r2 < 9-2; r2++) {
      Group g2 = b.candidateCells(v, b.getRow(r2));
      if (g2 == null || g2.getSize() > 4) continue;
      int[] cols2 = merge(cols1, getColumns(g2));
      if (cols2.length > 4) continue;

      for (int r3 = r2+1; r3 < 9-1; r3++) {
      Group g3 = b.candidateCells(v, b.getRow(r3));
      if (g3 == null || g3.getSize() > 4) continue;
      int[] cols3 = merge(cols2, getColumns(g3));
      if (cols3.length > 4) continue;

      for (int r4 = r3+1; r4 < 9; r4++) {
      Group g4 = b.candidateCells(v, b.getRow(r4));
      if (g4 == null || g4.getSize() > 4) continue;
      int[] cols4 = merge(cols3, getColumns(g4));
      if (cols4.length != 4) continue;

      //System.out.println("possible yellyfish " + v + " in " + "r"+(r1+1) + "r"+(r2+1) + "r"+(r3+1)+ "r"+(r4+1));

      Group g5 = b.candidateCells(v, b.getCol(cols4[0]));
      Group g6 = b.candidateCells(v, b.getCol(cols4[1]));
      Group g7 = b.candidateCells(v, b.getCol(cols4[2]));
      Group g8 = b.candidateCells(v, b.getCol(cols4[3]));

      if (solvedYellyfish(v, g1, g2, g3, g4, g5, g6, g7, g8)) return true;
    }}}}

    for (int c1=0; c1 < 9-3; c1++) {
      Group g1 = b.candidateCells(v, b.getCol(c1));
      if (g1 == null || g1.getSize() > 4) continue;
      int[] rows1 = getRows(g1);

      for (int c2 = c1+1; c2 < 9-2; c2++) {
      Group g2 = b.candidateCells(v, b.getCol(c2));
      if (g2 == null || g2.getSize() > 4) continue;
      int[] rows2 = merge(rows1, getRows(g2));
      if (rows2.length > 4) continue;

      for (int c3 = c2+1; c3 < 9-1; c3++) {
      Group g3 = b.candidateCells(v, b.getCol(c3));
      if (g3 == null || g3.getSize() > 4) continue;
      int[] rows3 = merge(rows2, getRows(g3));
      if (rows3.length > 4) continue;

      for (int c4 = c3+1; c4 < 9; c4++) {
      Group g4 = b.candidateCells(v, b.getCol(c4));
      if (g4 == null || g4.getSize() > 4) continue;
      int[] rows4 = merge(rows3, getRows(g4));
      if (rows4.length != 4) continue;

      //System.out.println("possible yellyfish " + v + " in " + "c"+(c1+1) + "c"+(c2+1) + "c"+(c3+1)+ "c"+(c4+1));

      Group g5 = b.candidateCells(v, b.getRow(rows4[0]));
      Group g6 = b.candidateCells(v, b.getRow(rows4[1]));
      Group g7 = b.candidateCells(v, b.getRow(rows4[2]));
      Group g8 = b.candidateCells(v, b.getRow(rows4[3]));

      if (solvedYellyfish(v, g1, g2, g3, g4, g5, g6, g7, g8)) return true;
    }}}}
  }
  return false;
}

// XY-WING

private boolean adj(Cell c1, Cell c2) {
  return b.getBox(c1) == b.getBox(c2) || b.getRow(c1) == b.getRow(c2) || b.getCol(c1) == b.getCol(c2);
}

private boolean solvedXYZ(Cell xy, Cell xz, Cell yz) {

  byte z = xz.getCandidate(0);
  if (!yz.presentCandidate(z)) z = xz.getCandidate(1);

  Group gz = new Group(3, "");
  b.intersect(xz.getNeighbors(), yz.getNeighbors(), gz);

  if (b.presentCandidate(z, gz) == null) return false;

  String info = "xy-wing " + z + " in " + xy.getName() + "," + xz.getName() + "," + yz.getName();

  //System.out.println(info);
  //showCandidates();

  b.excludeCandidate(gz, z);
  xy_wing++; solve(info); xy_wing--;
  b.includeCandidate(gz, z);

  return true;
}

private boolean solveXYWing() {

  Group all = b.getAll();

  for (int i=0; i < 9*9-2; i++) {
    Cell c1 = all.getCell(i);
    if (c1.numberOfCandidates() != 2) continue;

    byte v1 = c1.getCandidate(0);
    byte v2 = c1.getCandidate(1);

    for (int j=i+1; j < 9*9-1; j++) {
      Cell c2 = all.getCell(j);
      if (c2.numberOfCandidates() != 2) continue;

      byte a = c2.getCandidate(0);
      byte b = c2.getCandidate(1);
      byte v3, v3_2;

      if ((a == v1 || a == v2) && b != v1 && b != v2) { v3 = b; v3_2 = a;}
      else if (a != v1 && a != v2 && (b == v1 || b == v2)) { v3 = a; v3_2 = b; }
      else continue;
      if (v3_2 == v1) v3_2 = v2; else v3_2 = v1;

      for (int k=j+1; k < 9*9; k++) {
        Cell c3 = all.getCell(k);
        if (c3.numberOfCandidates() != 2) continue;

        if (!c3.presentCandidate(v3) || !c3.presentCandidate(v3_2)) continue;

        // XY, XZ, YZ found

        if (adj(c1, c2) && adj(c1, c3) && !adj(c2, c3)) {
          if (solvedXYZ(c1, c2, c3)) return true;
        } else if (adj(c1, c2) && !adj(c1, c3) && adj(c2, c3)) {
          if (solvedXYZ(c2, c3, c1)) return true;
        } else if (!adj(c1, c2) && adj(c1, c3) && adj(c2, c3)) {
          if (solvedXYZ(c3, c1, c2)) return true;
        }
      }
    }
  }
  return false;
}

// GUESS

private void solveGuess(Cell cell) {
  byte[] candidates = cell.getAllCandidates();
  for (int i=0; i < candidates.length; i++) {
    byte value = candidates[i];

    String info = "guess " + value + " in " + cell.getName();

    b.move(cell, value);
    guess++; solve(info); guess--;
    b.undo(cell, value); 
  }
}

// SOLVE

private void solve() {

  // stop solving when multiple sulutions
  if (result == Results.MULTIPLE) {
    return;
  }

  // solution found when all cells are filled
  if (b.solved()) {
    report(Results.SOLUTION);
    return;
  }

  // get candidate cell to proceed
  Cell cell = b.getCellWithFewestCandidates(b.getAll());

  // backtrack when unsolved cell has no candidates
  if (cell.numberOfCandidates() == 0) {
    return;
  }

  if (solveNakedSingle(cell)) return;
  if (solveHiddenSingle())    return;
  if (solveNakedPair())       return;
  if (solveHiddenPair())      return;
  if (solveNakedTriple())     return;
  if (solveHiddenTriple())    return;
  if (solveNakedQuad())       return;
  if (solveHiddenQuad())      return;
  if (solveLockedCandidate()) return;
  if (solveXWing())           return;
  if (solveSwordfish())       return;
  if (solveYellyfish())       return;
  if (solveXYWing())          return;

  if (trial_and_error) {
    if (guess == 0 && b.impossible()) {
      report(Results.IMPOSSIBLE);
      return;
    }
    solveGuess(cell);

    if (guess == 0 && result == Results.NOTHING) {
      report(Results.UNSOLVABLE);
    }
    return;
  }

  report(Results.UNFINISHED);
}

private void solve(String info) {
  history[depth] = info;
  depth++; solve(); depth--;
}

public void solve(byte[][] grid, String[][] cand) {
  b.setup(grid, cand);
  if (show_input) showCandidates();
  solve();
  if (show_result) System.out.println("" + result);
}

// MAIN

public static void main(String[] args) throws IOException {
  byte grid[][] = new byte[9][9];
  String cand[][] = new String[9][9];

  String options  = (args.length > 0) ? args[0] : "hiorst";
  show_history    = options.contains("h");
  show_input      = options.contains("i");
  show_output     = options.contains("o");
  show_result     = options.contains("r");
  show_summary    = options.contains("s");
  trial_and_error = options.contains("t");

  readInput(grid, cand);
  showInput(grid, cand);
  new Sudoku().solve(grid, cand);
}}


