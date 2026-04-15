namespace My_Game
{
    // 1. Перечисление для ID квестов
    public enum QuestID
    {
        KillStrongMonsters = 1,
    }

    public class Quest
    {
        public QuestID Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;

        // Глобальный список активных квестов игрока (как инвентарь)
        public static List<Quest> ActiveQuests = new();

        // Конструктор
        public Quest(QuestID id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        // 2. Метод вывода информации о квесте
        public void QuestInfo()
        {
            Console.WriteLine($"\n=== Квест: {Name} ===");
            Console.WriteLine($"Цель: {Description}");
            Console.WriteLine($"Статус: {(IsCompleted ? "Выполнен" : "В процессе")}");

            // Специфичная логика вывода для квеста на убийство
            if (Id == QuestID.KillStrongMonsters && !IsCompleted)
            {
                // LINQ запрос: 
                // Where - фильтрует сильных
                // Take(5) - берет максимум 5 первых из списка
                var targets = Monster.MonsterList
                    .Where(m => m.PowerScore > 150)
                    .Take(5)
                    .ToList();
            }
        }

        // 3. Твоя заготовка для проверки прогресса
        // Заметь: параметры точно такие же, как в OnStep (int y, int x, ref bool cancel)
        public void CheckProgress(int targetY, int targetX, ref bool cancelMove)
        {
            // Если квест уже сдан/выполнен, нам не нужно ничего проверять
            //if (IsCompleted)
            //{
            //    //хочу удалить квест из ActiveQuest
            //    return;
            //}

            switch ((int)Id)
            {
                case 1:
                    var StrongMonster = Monster.MonsterList
                                    .Where(m => m.PowerScore >= 150)
                                    .ToList();
                    if (StrongMonster.Count == 0)
                    {
                        this.IsCompleted = true;
                        Execution.Player_1.gold += 30;

                        // Выводим красивое сообщение о завершении
                        Console.WriteLine($"\nПОЗДРАВЛЯЕМ! Квест '{Name}' выполнен!");
                        Console.WriteLine("Награда: +30 золота.");
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();

                        // === ОЧИСТКА ===
                        // 1. Удаляем сам себя из списка активных квестов
                        ActiveQuests.Remove(this);

                        // 2. ВАЖНО: Отписываем этот конкретный метод от события шага, 
                        // чтобы игра больше не тратила ресурсы на его проверку!
                        Execution.OnStep -= CheckProgress;
                    }
                    break;
            }
        }
    }
}
