module NimsTest

    open NUnit.Framework
    open FsUnit
    open SimpleGames
    open MCTS.Interfaces

    [<Test>]
    let ``Test that with one chips to pick, there is only one move possible``() =
        let nims = Nims.Nims(1) :> IGameState
        nims.GetMoves()
        |> Seq.toList 
        |> should haveLength 1

    [<Test>]
    let ``Test that with two chips to pick, there is two possible moves``() =
        let nims = Nims.Nims(2) :> IGameState
        nims.GetMoves()
        |> Seq.toList 
        |> should haveLength 2

    [<Test>]
    let ``Test that with three chips to pick, there is three possible moves``() =
        let nims = Nims.Nims(3) :> IGameState
        nims.GetMoves()
        |> Seq.toList
        |> should haveLength 3

    [<Test>]
    let ``Test that with four chips to pick, there is three possible moves``() =
        let nims = Nims.Nims(4) :> IGameState
        nims.GetMoves()
        |> Seq.toList 
        |> should haveLength 3


    [<Test>]
    let ``Check we got good winner``() =
        let nims = Nims.Nims(1, Nims.Player.Player1) :> IGameState
        let moves = 
            nims.GetMoves()
            |> Seq.toList
        let nims2 = moves.Head.DoMove()
        nims2.GetResult(Nims.Player1)
        |> should equal MCTS.Enum.EGameFinalStatus.GameWon

    [<Test>]
    let ``Check we got good winner also with Player2``() =
        let nims = Nims.Nims(1, Nims.Player.Player2) :> IGameState
        let moves = 
            nims.GetMoves()
            |> Seq.toList
        let nims2 = moves.Head.DoMove()
        nims2.GetResult(Nims.Player2)
        |> should equal MCTS.Enum.EGameFinalStatus.GameWon

    [<Test>]
    let ``Check we got good loser``() =
        let nims = Nims.Nims(1, Nims.Player.Player1) :> IGameState
        let moves = 
            nims.GetMoves()
            |> Seq.toList
        let nims2 = moves.Head.DoMove()
        nims2.GetResult(Nims.Player2)
        |> should equal MCTS.Enum.EGameFinalStatus.GameLost

    [<Test>]
    let ``Check we got good loser also with Player2``() =
        let nims = Nims.Nims(1, Nims.Player.Player2) :> IGameState
        let moves = 
            nims.GetMoves()
            |> Seq.toList
        let nims2 = moves.Head.DoMove()
        nims2.GetResult(Nims.Player1)
        |> should equal MCTS.Enum.EGameFinalStatus.GameLost

    [<Test>]
    let ``Check CurrentPlayer() when starting``() =
        let nims = Nims.Nims(1, Nims.Player.Player1)  :> IGameState
        nims.CurrentPlayer()
        |> should equal Nims.Player1


    [<Test>]
    let ``Check CurrentPlayer() after a move``() =
        let nims = Nims.Nims(1, Nims.Player.Player1)  :> IGameState
        let moves = 
            nims.GetMoves()
            |> Seq.toList
        let nims2 = moves.Head.DoMove()
        nims2.CurrentPlayer()
        |> should equal Nims.Player2

    [<Test>]
    let ``Check random game when only one move left is a win``() =
        let nims = Nims.Nims(1, Nims.Player.Player1)  :> IGameState
        nims.PlayRandomlyUntilTheEnd(Nims.Player1)
        |> should equal MCTS.Enum.EGameFinalStatus.GameWon

        nims.PlayRandomlyUntilTheEnd(Nims.Player2)
        |> should equal MCTS.Enum.EGameFinalStatus.GameLost


    [<Test>]
    let ``Check winner after a move``()=
        let nims = Nims.Nims(2, Nims.Player.Player1)  :> IGameState
        let moves = nims.GetMoves();
        let move = 
            moves
            |> Seq.find (fun m -> m.Name = "1")
        let nims2 = move.DoMove() 
        nims2.PlayRandomlyUntilTheEnd(Nims.Player2)
        |> should equal MCTS.Enum.EGameFinalStatus.GameWon

        nims2.PlayRandomlyUntilTheEnd(Nims.Player1)
        |> should equal MCTS.Enum.EGameFinalStatus.GameLost


    [<Test>]
    let ``Check Game used in MCTS Test should be a win for Player1``()=
        let pickMove name (seq : IMove seq) =
            seq
            |> Seq.find (fun item -> item.Name = name)
        let nims = Nims.Nims(5, Nims.Player.Player1)  :> IGameState
        let moves = nims.GetMoves();
        // AI Player1 *should* pick this move
        let move = 
            moves
            |> pickMove "4"
        // AI Player2 can pick any move Player1 can win next turn
        let nims2 = move.DoMove()
        let moves2 =
            nims2.GetMoves()
        // any move
        let n = System.Random().Next(1,4)
        let randomMove = 
            moves2
            |> pickMove (n.ToString())
        let nims3 = randomMove.DoMove()            
        nims3.GetMoves()
        |> Seq.exists (fun move -> move.Name = "0") // win move
        |> should be True




