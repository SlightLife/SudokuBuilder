using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour{
    // public Tile[,] tiles;
    // public Square[,] squares;
    public int totalAttemps;
    static List<int> oneToNine = new List<int>{1,2,3,4,5,6,7,8,9};
    List<int> forUse;
    int[,] all;

    #region prev attempts
    // [EditorButton]
        //didn't work because took way too long, not efficient
    // public void Indexes(){
    //     if(++totalAttemps>10)return;
    //     tiles = new Tile[9,9];
    //     List<int> holding = new List<int>{0,1,2,3,4,5,6,7,8}, xList, yList;
    //     List<(int,int)> all = new List<(int, int)>();
    //     // for(int i=0;i<9;i++){
    //     //     for(int j=0;j<9;j++){
    //     //         all.Add((i,j));
    //     //     }
    //     // }
    //     bool failed=false;
    //     for(int i=1;i<10;i++){
    //         if(failed)break;
    //         xList = holding.ToList(); yList = holding.ToList();
    //         List<(int,int)> added = new List<(int, int)>();
    //         int bigTries=0;
    //         for(int j=0;j<9;j++){
    //             int x, y, smolTries=0;
    //             do{
    //                 x = xList.Random();
    //                 y = yList.Random();
    //             }while(all.Contains((x,y)) && ++smolTries<50);
    //             // if(smolTries.__Log("smolTries: ") >= 999){
    //             if(smolTries >= 50){
    //                 i--;
    //                 foreach(var v in added)all.Remove(v);
    //                 bigTries++;
    //                 // if(bigTries.__Log("bigTries: ")>1000){
    //                 if(bigTries>=10){
    //                     GenerateABoard();
    //                     failed=true;
    //                     return;
    //                 }
    //             }else{
    //                 added.Add((x, y));
    //                 all.Add((x, y));
    //                 xList.Remove(x);
    //                 yList.Remove(y);
    //             }
    //         }
    //     }
    //     if(!failed){
    //         for(int i=0;i<9;i++){
    //             for(int j=1;j<10;j++){
    //                 var ints = all.PopAt(0);
    //                 tiles[ints.Item1, ints.Item2] = new Tile(j, ints);
    //             }
    //         }
    //         string together = "";
    //         for(int i=0;i<9;i++){
    //             for(int j=0;j<9;j++){
    //                 together += tiles[i,j].num;
    //             }
    //             together+="\n";
    //         }
    //         Debug.Log(together);
    //     }
    // }

    //this combo was really efficient, even up to 100000 tries per square, but even that was not enough

    // [EditorButton]
    // public void Squares(){
    //     squares = new Square[3,3];
    //     //gotta fill em empty to not null ref
    //     for(int i=0;i<9;i++)squares[i/3,i%3] = new Square();

    //     List<int> holding = new List<int>(){1,2,3,4,5,6,7,8,9};
    //     List<string> all = new List<string>(){"","","","","","","","",""};

    //     for(int sqIndex=0;sqIndex<9;sqIndex++){
    //         int tries=0;
    //         Square sq;
    //         do sq = new Square().SetMeUp(holding=holding.Shuffle());
    //         while(!CheckRelevantSquares(sq, sqIndex) && ++tries<totalAttemps);
    //         Debug.Log($"making square {sqIndex}, took {tries} tries");
    //         squares[sqIndex/3, sqIndex%3] = sq;
            
    //         // for(int row=0;row<3;row++){
    //         //     string s = $"{holding[row*3]}{holding[row*3+1]}{holding[row*3]+2}";
    //         //     all[row + (sqIndex/3)*3]+=s;
    //         // }
    //     }
    //     for(int sqCol=0;sqCol<3;sqCol++){
    //         for(int sqRow=0;sqRow<3;sqRow++){
    //             for(int localRow=0;localRow<3;localRow++)all[sqRow*3+localRow]+=string.Join("",squares[sqRow,sqCol].InRow(localRow));
    //         }
    //     }
    //     Debug.Log("\n"+string.Join("\n", all));
    // }
    // bool CheckRelevantSquares(Square sq, int index){
    //     int x = index/3, y = index%3;
    //     bool good=true;
    //     //check vertical
    //     //ex: 2
    //     //oox
    //     //oox
    //     //oox
    //     for(int i=0;i<3;i++){
    //         if(x==i || !good)continue;
    //         good = squares[i,y].CheckAgainstBox(sq, true);
    //     }
    //     //check horizontal
    //     for(int i=0;i<3;i++){
    //         if(y==i || !good)continue;
    //         good = squares[x,i].CheckAgainstBox(sq, false);
    //     }
    //     return good;
    // }

    #endregion


    public void BuildNewBoard(){
        
    }

    [EditorButton]
    void Lines(){
        all = new int[9,9];
        forUse = new List<int>();
        List<int> copy;
        int totalTries=0;

        //going by column, left to right
        for(int col=0;col<9;col++){
            copy = oneToNine.Shuffle();
            if(totalTries>=totalAttemps)continue;

            //then each number (by index of the randomized list)
            for(int num=0;num<9;num++){     //although this iteration is specifically for the 1-9 nums, they're coming out of a list, so it has to be a zero based index
                //then trying each row until it succeeds
                for(int row=0;row<9;row++){
                    if(all[col, row]==0 && PassesTests(copy[num], col, row)){
                        all[col, row] = copy[num];
                        break;
                    }
                }
            }
            if(!AllColumnAssigned(col)){
                ClearColumn(col);
                col--;
                totalTries++;
            }
        }

        string res = "\n";
        for(int y=0;y<9;y++){
            for(int x=0;x<9;x++){
                res+=all[x,y];
            }
            res+="\n";
        }
        Debug.Log($"attempts: {totalTries}, res: {res}");
    }
    //this /3 *3 shit is to round down then go back up, since being at [0,8] would be 8/3 (2) * 3, which is square 6
    bool PassesTests(int num, int col, int row){
        int square = (row/3)*3 + col/3;
        return !InRow(row).Contains(num) && !InSquare(square).Contains(num);
    }
    bool AllColumnAssigned(int column){
        for(int i=0;i<9;i++){
            if(all[column,i]==0)return false;
        }
        return true;
    }
    void ClearColumn(int column){
        for(int i=0;i<9;i++)all[column,i]=0;
    }
    List<int> InRow(int index){
        forUse.Clear();
        for(int i=0;i<all.GetLength(0);i++)forUse.Add(all[i, index]);
        return forUse;
    }
    List<int> InSquare(int index){
        forUse.Clear();
        //for 4: 4%3=1, 4/3=1
        int indX = index%3, indY = index/3;
        indX*=3;
        indY*=3;
        for(int x=indX;x<indX+3;x++){
            for(int y=indY;y<indY+3;y++){
                forUse.Add(all[x,y]);
            }
        }
        return forUse;
    }
}
