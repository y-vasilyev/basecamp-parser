// Learn more about F# at http://fsharp.org

open System
open FSharp.Data
open System.Text.RegularExpressions
open System
open System.IO

type Basecamp = JsonProvider<"D:\\Codefest\\todos.json">


let grabEmail (s:option<string>) :string =
    match s with
      | Some data ->
                let matches = Regex.Matches(data, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
                if Seq.isEmpty matches 
                then ""
                else
                            matches
                            |> Seq.cast<Match>
                            |> Seq.map (fun m -> m.Value)
                            |> Seq.head
                            
      | None -> ""
   
let grabName (s:string):string =
    if s.IndexOf("(") > 0 then
        s.Substring(0, s.IndexOf("(")).Trim();
    else s

type RecordTest = { Description: string; Title: string }

let readJson :  System.Collections.Generic.List<RecordTest> =
  let files = Directory.GetFiles "D:\\Codefest\\Data"

  let results = new System.Collections.Generic.List<RecordTest>()
 
  for file in files do
    let content = Basecamp.Load file
    

    let result = content |> Seq.map(fun issue -> 
                                 let email = grabEmail issue.Description
                                 let name = grabName issue.Title
                                 {Description = email; Title = name}
                              )
                              |> Seq.toArray
    results.AddRange result
  
  results


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
    
    let todos = 
        //Basecamp.GetSamples()
        readJson
        |> Seq.filter (fun issue ->  
                    let name = issue.Title
                    List.contains name selectedSpeakers |> not)
        |> Seq.map(fun issue -> 
                 let email = issue.Description
                 let name = issue.Title
                 name + "," + email
                 )
        |> Seq.distinct

    
    File.AppendAllLines("D:\\Codefest\\decline.csv", todos)
    for issue in todos do
        printfn "%s" issue
    Console.ReadLine() |> ignore
    0 