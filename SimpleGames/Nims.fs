namespace SimpleGames

module Nims =
    open MCTS.Interfaces
    open MCTS.Enum

    let random = System.Random()

    type Player =
        | Player1 
        | Player2 
        interface IPlayer with
            member this.Name = 
                match this with
                | Player1 -> "Player1"
                | Player2 -> "Player2"
    
    type Move (chips) =
        interface IMove with
            member this.Name = chips.ToString()

                

    and Nims( tokenNumber : int, currentPlayer : IPlayer) =
        do 
            if (tokenNumber < 0) then
                invalidArg "tokenNumber" "Should be positive or equal to 0"

        let nextPlayer (player : IPlayer)=
            match player.Name with
            | "Player1" -> Player2 :> IPlayer
            | "Player2" -> Player1 :> IPlayer
            | _ -> invalidOp "Invalid player name %A" player.Name       
        
        /// used when token is 0
        /// previous player won
        let getResult (player : IPlayer) player2 =
            if ( player2 = player) then
                EGameFinalStatus.GameLost
            else
                EGameFinalStatus.GameWon

        new(tokenNumber) =
            Nims(tokenNumber, Player.Player1)

        interface IGameState with
            member this.CurrentPlayer() =
                currentPlayer

            member this.GetMoves() =
                let build n =
                    seq{for i in 1..n do yield Move((tokenNumber - i), nextPlayer currentPlayer) :> IMove}
                if (tokenNumber - 3 >= 0) then
                    build 3
                elif (tokenNumber - 2 >= 0) then
                    build 2
                elif (tokenNumber - 1 >= 0) then
                    build 1
                else
                    Seq.empty

            member this.GetResult player =
                match tokenNumber with
                | x when x = 0 ->
                    getResult player currentPlayer
                | _ -> 
                    invalidOp "Can't give result, game is not finished"
                
            member this.PlayRandomlyUntilTheEnd  =
                let rec play token p =
                    match token with
                    | x when x = 0 ->
                        ()
                    | _ -> 
                        let min = min 4 (token + 1)
                        let n = random.Next(1, min)
                        play (token - n) (nextPlayer p)
                play tokenNumber currentPlayer  
                        

