``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 8.1 (6.3.9600.0)
AMD FX(tm)-8350 Eight-Core Processor, 1 CPU, 8 logical and 4 physical cores
Frequency=3919441 Hz, Resolution=255.1384 ns, Timer=TSC
.NET Core SDK=2.1.300
  [Host]     : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


```
|                               Method |          Mean |         Error |        StdDev |
|------------------------------------- |--------------:|--------------:|--------------:|
|                     InsertRangeItems | 188,551.33 us |   805.5416 us |   628.9138 us |
|                    SingleInsertItems | 237,413.56 us | 5,622.7801 us | 6,249.7046 us |
|                      OverwriteValues | 237,384.72 us | 2,527.4861 us | 2,240.5503 us |
|             IterateThroughEveryEntry |   1,270.81 us |    19.7085 us |    18.4354 us |
|                      GetValueOfFirst |   1,378.73 us |    22.8847 us |    21.4063 us |
|                     GetValueOfMiddle |      13.69 us |     0.1714 us |     0.1603 us |
|                        GetValueOfEnd |   1,366.08 us |    26.9650 us |    25.2231 us |
| IterateThroughEveryEntryAndReadValue |  82,201.33 us |   731.7281 us |   684.4588 us |
|                    CleanFromDatabase | 281,773.34 us | 3,347.8844 us | 3,131.6128 us |
|                      CleanToDatabase | 277,571.14 us | 2,478.2186 us | 2,318.1270 us |
