using System;

class Car {
    public string Color;
    public int Speed;
    public string Name;

    public Car(){
        Color = "White";
        Speed = 200;
        Name = "Mercedes";
    }

    public Car(string color, int speed, string name){
        Color = color;
        Speed = speed;
        Name = name;
    }

    public string CarInfo()
    {
        return $"Color - {Color}, Speed - {Speed}, Name - {Name}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Car car2 = new Car();
        Car car3 = new Car("Black", 270, "Toyota");
        
        Console.WriteLine(car2.CarInfo());
        Console.WriteLine(car3.CarInfo());

        // Ожидание ввода пользователя перед завершением программы
        Console.ReadLine();
    }
}
