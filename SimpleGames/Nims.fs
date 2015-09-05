namespace SimpleGames

module Nims =
    open MCTS.Interfaces

    type Player =
        | Player1
        | Player2 

    type PlayerClass (name) =
        interface IPlayer with
            member this.Name = name

    let Player1 = 
        PlayerClass("player1") :> IPlayer
    let Player2 = 
        PlayerClass("player2") :> IPlayer
    
    type Move (chips, next) =
        interface IMove with
            member this.Name = chips.ToString()

            member this.DoMove() =
                Nims(chips, next) :> IGameState
                

    and Nims( tokenNumber : int, currentPlayer) =
        do 
            if (tokenNumber <= 0) then
                invalidArg "tokenNumber" "Should be strictly positive"

        let nextPlayer =
            function
            | Player1 -> Player.Player2
            | Player2 -> Player.Player1
        
        new(tokenNumber) =
            Nims(tokenNumber, Player.Player1)

        interface IGameState with
            member this.CurrentPlayer() =
                match currentPlayer with
                | Player1 -> Player1
                | Player2 -> Player2

            member this.GetMoves() =
                let next = 
                    currentPlayer
                    |> nextPlayer
                let build n =
                    seq{for i in 1..n do yield Move((tokenNumber - i), next) :> IMove}
                if (tokenNumber - 3 > 0) then
                    build 3
                elif (tokenNumber - 2 > 0) then
                    build 2
                elif (tokenNumber - 1 > 0) then
                    build 1
                else
                    Seq.empty

            member this.PlayRandomlyUntilTheEnd  player =
            // goto BED
                MCTS.Enum.EGameFinalStatus.GameWon


