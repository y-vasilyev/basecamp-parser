// Learn more about F# at http://fsharp.org

open System
open FSharp.Data
open System.Text.RegularExpressions
open System
open System.IO

type Basecamp = JsonProvider<"D:\\Codefest\\todos.json">

// module usage
[<AutoOpen>]
module IssueParsing =
  // const usage
  [<Literal>]
  let private EmailTemplate = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
  
  type private Issue =
    { Description : string
      Title       : string
    }

  // active pattern usage
  let private (|RegexMatch|_|) pattern input =
    if not (isNull input) then // null guard
      Regex.Matches(input, pattern)
      |> Seq.cast<Match>
      |> Seq.map (fun m -> m.Value)
      |> Seq.tryHead
    else
      None

  // discrimination union
  type Contact = Email of string | NotSpecified

  // active pattern usage
  let private (|Name|_|) (s: string) =
      if not (isNull s) then // null guard
        let bracketPlace = s.IndexOf("(")
        if bracketPlace > 0
          then s.Substring(0, bracketPlace).Trim() |> Some
          else Some s
      else
        None

  type Author =
    { Name    : string
      Contact : Contact
    }

  // Either monad usage
  let private parseAuthor = function // pattern matching short syntax
    | { Title = Name name; Description = RegexMatch EmailTemplate address } ->
        Ok { Name = name; Contact = Email address }
    
    | { Title = Name name } ->
        Ok { Name = name; Contact = NotSpecified }

    | _ ->
        Error "null author name"

  // compose usage
  let readAuthorsFromJson readFile =
    Directory.GetFiles
    // >> - compose
    >> Seq.collect (readFile >> Seq.map parseAuthor) // Seq.collect == SelectMany

[<EntryPoint>]
let main argv =

    let selectedSpeakers = [
        "Тоболь Александр Александрович"
        "Панченко Иван Евгеньевич"
        "Фастов Иван Владимирович"
        "Чечель Максим Михайлович"
        "Чапоргин Антон"
        "Неволин Роман"
        "Кочепасов Антон Алексеевич"
        "Абилов Вагиф Галлиевич"
        "Михаил Ярийчук"
        "Кирпичников Алексей Николаевич"
        "Аршинов Максим Вячеславович"
        "Dylan Beattie"
        "Валеев Тагир Фаридович"
        "Финкельштейн Павел Михайлович"
        "Александров Дмитрий Вячеславович"
        "Липский Никита Валерьевич"
        "Александров Дмитрий Вячеславович"
        "Кошелев Григорий Николаевич"
        "Паньгин"
        "Толстопятов"
        "Плизга"
    ]

    // https://3.basecamp.com/3152562/buckets/8281413/todolists/1303394617/todos.json?completed=true
    // https://3.basecamp.com/3152562/buckets/8281413/todolists/1303394617/todos.json?completed=true&page=2
    // https://3.basecamp.com/3152562/buckets/8281413/todolists/1303394617/groups.json
    // https://github.com/basecamp/bc3-api/blob/master/sections/todolist_groups.md#to-do-list-groups
    
    let authorResults =
      readAuthorsFromJson Basecamp.Load "D:\\Codefest\\Data"
    
    let todos = 
        //Basecamp.GetSamples()
        authorResults
        |> Seq.filter (function // pattern matching short syntax
          | Ok author ->
              not (selectedSpeakers |> List.contains author.Name)
          | _ ->
              false)
        |> Seq.map (sprintf "%A") // predefined formatting
        |> Seq.distinct

    
    File.AppendAllLines("D:\\Codefest\\decline.csv", todos)

    todos
    |> Seq.iter (printfn "%s")

    Console.ReadLine() |> ignore
    0 