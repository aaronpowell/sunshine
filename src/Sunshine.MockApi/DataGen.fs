module DataGen

open System

let rand min max =
    let random = Random()
    random.NextDouble() * (max - min) + min

let epoch = DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)