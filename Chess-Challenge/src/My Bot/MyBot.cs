using ChessChallenge.API;

public class MyBot : IChessBot
{
    Timer globalTimer;
    public Move Think(Board board, Timer timer)
    {
        globalTimer = timer;
        return search(board,4);
        
    }
    
    private double eval(Board board)
    {
        
        double i = 0;
        PieceList[] pl = board.GetAllPieceLists();
        
        i += pl[0].Count - pl[6].Count;
        i += (pl[1].Count + pl[2].Count)*3 - (pl[7].Count + pl[8].Count)*3;
        i += pl[3].Count*5 - pl[9].Count*5;
        i += pl[4].Count*9 - pl[10].Count*9;

        if (board.IsDraw()) i = 0;
        if (board.IsInCheckmate() && board.IsWhiteToMove){
            i = -9999;
        } 
        if (board.IsInCheckmate() && !board.IsWhiteToMove){
            i = 9999;
        }
        System.Random random = new System.Random();
        return i + random.NextDouble()*0.01-random.NextDouble()*0.01;
    }

    private System.Span<Move> sortMoves(System.Span<Move> moves){
        System.Span<Move> returnArray = new System.Span<Move>(new Move[moves.Length]);

        int i = 0;
        int j = moves.Length - 1;
        foreach(Move m in moves){
            if(m.IsCapture){
                returnArray[i] = m;
                i++;
            }
            else{
                returnArray[j] = m;
                j--;
            }
        }

        return returnArray;
    }
    private Move search(Board board, int depth)
    {
        System.Span<Move> moves = stackalloc Move[256];
        board.GetLegalMovesNonAlloc(ref moves);

        if(moves.Length == 0) return Move.NullMove;
        Move bestMove = moves[0];
        double currentMin = 99999;
        double currentMax = -99999;
        moves = sortMoves(moves);
        foreach (Move m in moves){
            board.MakeMove(m);
            double result = alphabeta(board, depth-1,-99999,99999, board.IsWhiteToMove);
            board.UndoMove(m);
            if (board.IsWhiteToMove){
                if (result > currentMax){
                    currentMax = result;
                    bestMove = m;
                }
            }
            else{
             if (result < currentMin){
                    currentMin = result;
                    bestMove = m;
                }
            }
            // System.Console.WriteLine(m);
            //System.Console.WriteLine(result);  
                     
        }
        return bestMove;
    }
    private double alphabeta(Board board, int depth, double alpha, double beta, bool isMaximizing)
    {
        System.Span<Move> moves = stackalloc Move[256];
        board.GetLegalMovesNonAlloc(ref moves);
        if(depth <= 0 || moves.Length == 0){
            return eval(board);
        }


        //moves = sortMoves(moves);
        
        if(isMaximizing){ //white
            double maxValue = -99999;
            foreach (Move m in moves) 
            {
                board.MakeMove(m);
                
                double result = alphabeta(board, (m.IsCapture && depth == 1) ? depth : depth -1, alpha, beta, !isMaximizing); //todo
                
                maxValue = System.Math.Max(maxValue, result);
                alpha = System.Math.Max(alpha, result);
                
                if(beta <= alpha){
                    board.UndoMove(m);
                    break;
                }
                else{
                    board.UndoMove(m);
                }
                
            }
            return maxValue;
        }
        else{ //black
            double minValue = 99999;
            foreach (Move m in moves) 
            {
                board.MakeMove(m);
           
                
                double result = alphabeta(board, (m.IsCapture && depth == 1) ? depth : depth -1, alpha, beta, !isMaximizing); //todo


                minValue = System.Math.Min(minValue, result);
                beta = System.Math.Min(beta, result);
                if(beta <= alpha){
                    board.UndoMove(m);
                    break;
                }else{
                    board.UndoMove(m);
                }
                
            }
            return minValue;
        }
    }
}

