# OOP-Lab2

Виконала Пуха Софія, К-26

gantt
    dateFormat  s
    axisFormat  %S
    title Схема виконання (Варіант 15)
    
    section Main Thread
    A2 (Slow)           :active, a2, 0, 7s
    B (Quick)           :crit, b, after a2 a1, 1s
    D (Quick)           :crit, d, after b, 1s
    
    section Thread 2 (Async)
    A1 (Slow)           :active, a1, 0, 7s
    
    section Thread 3 (Async)
    A3 (Quick)          :done, a3, 0, 1s
    A4 (Quick)          :done, a4, after a3, 1s
    C  (Quick)          :done, c, after a4, 1s
