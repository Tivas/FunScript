﻿[<ReflectedDefinition>]
module Program

open FunJS
open FunJS.TypeScript

// TODO: constructors, op_Apply, param arrays, optional arguments...
type j = FunJS.TypeScript.Api< @"C:\src\FunScript\Examples\Typings\jquery.d.ts" >
type jui = FunJS.TypeScript.Api< @"C:\src\FunScript\Examples\Typings\jqueryui.d.ts" >

[<JSEmit("return $(document);")>]
let document() = Unchecked.defaultof<j.JQuery>

[<JSEmit("alert({0});")>]
let alert(msg:string) = ()

[<JSEmit("console.log({0});")>]
let log(msg:string) = ()

[<JSEmit("return Math.floor((Math.random()*{1})+{0});")>]
let random(lowerInclusive:int, upperInclusive:int) = 0

type SuitType =
   | Clubs
   | Spades
   | Hearts
   | Diamonds

   member t.Name =
      match t with
      | Clubs -> "Clubs"
      | Spades -> "Spades"
      | Hearts -> "Hearts"
      | Diamonds -> "Diamonds"

   static member All = [Clubs; Spades; Hearts; Diamonds]

type CardType =
   | Ace
   | King
   | Queen
   | Jack
   | Number of int

   member t.Name =
      match t with
      | Ace -> "Ace"
      | King -> "King"
      | Queen -> "Queen"
      | Jack -> "Jack"
      | Number n -> n.ToString()

   static member All =
      [  Ace
         King
         Queen
         Jack
         Number 10
         Number 9
         Number 8
         Number 7
         Number 6
         Number 5
         Number 4
         Number 3
         Number 2 
      ]

type Card =
 { Suit: SuitType
   Type: CardType }
   member card.Value =
      match card.Type with
      | Ace -> 11
      | King | Queen | Jack -> 10
      | Number n -> n

type Deck = Card list

let createDeck() =
   SuitType.All |> List.collect (fun suit ->
      CardType.All |> List.map (fun t ->
         { Suit = suit; Type = t }))

let shuffle(deck:Deck) =
   deck 
   |> List.map (fun card -> random(0, 100000), card)
   |> List.sortWith (fun (offA, _) (offB, _) -> offA - offB)
   |> List.map (fun (_, shuffledCard) -> shuffledCard)

type Player =
 { Hand: Card list }
   member player.Total = 
      player.Hand |> List.sumBy (fun card -> card.Value)
   member player.IsBust =
      player.Total > 21


let createGame() =
   let deck = createDeck() |> shuffle
   let rec printDeck (cards:Deck) =
      match cards with
      | [] -> ()
      | card::rest -> 
         log (card.Type.Name + " of " + card.Suit.Name)
         printDeck rest
   printDeck deck

let main() =
   document().ready(fun () -> createGame())

// Compile
let source = <@@ main() @@> |> Compiler.compile 
let sourceWrapped = sprintf "(function () %s)()" source
let filename = "blackjack.js"
System.IO.File.Delete filename
System.IO.File.WriteAllText(filename, sourceWrapped)
source|> printfn "%A"
System.Console.ReadLine() |> ignore