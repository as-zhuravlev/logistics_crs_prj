﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Npgsql;

namespace LogisticDB
{
    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public override string ToString()
        {
            return id.ToString() + " " + name;
        }
    }

    public class CargoType
    {
        public int id { get; set; }
        public string name { get; set; }
        public override string ToString()
        {
            return id.ToString() + " " + name;
        }
    }

    public class Car
    {
        public int id { get; set; }
        public string registration_number { get; set; }
        public int carmodel_id { get; set; }

        public override string ToString()
        {
            return id.ToString() + " " + registration_number + " " + carmodel_id;
        }
    }
    
    public class CarModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int cargotype_id { get; set; }
        public float payload { get; set; }
        public double price_buy { get; set; }
        public double price_sell { get; set; }
        public double price_empty_per_km { get; set; }
        public double price_full_per_km { get; set; }
        public double price_stand_per_day { get; set; }
    }
    
    public class CarView : Car
    {
        public string carmodel_name { get; set; }
        public string cargotype_name { get; set; }
        public int cargotype_id { get; set; }
        public float payload { get; set; }
        public double price_buy { get; set; }
        public double price_sell { get; set; }
        public double price_empty_per_km { get; set; }
        public double price_full_per_km { get; set; }
        public double price_stand_per_day { get; set; }
        public DateTime date_buy { get; set; }
        public DateTime? date_sell { get; set; }
    }
    
    public class CarModelCargoType : CarModel
    {
        public string cargotype_name { get; set; }
    }
  
    public class CarViewExpense : CarView
    {
        public double expense { get; set; }
    }

    public class Transaction
    {
        public int id { get; set; }
        public int car_id { get; set; }
        public float weight { get; set; }
        public int city_from { get; set; }
        public int city_to { get; set; }
        public DateTime date_from { get; set; }
        public DateTime date_to { get; set; }
    }

    public class TransactionView : Transaction
    {
       public string city_from_name { get; set; }
       public string city_to_name { get; set; }
       public double expense { get; set; }
       public double reward { get; set; }
       public int    distance { get; set; }
       public bool isUnprofitable  { get { return reward > 0 && expense >= reward; } }
       public bool isProfitable { get { return reward > 0 && expense < reward; } }

    }

    public class CarStayCoef
    {
        public string registration_number { get; set; }
        public string carmodel_name { get; set; }
        public string cargotype_name { get; set; }
        public float payload { get; set; }
        public float stay_coef { get; set; }
    }

    public class LogisticData
    {
        string ConnectStr { get { return "Server=127.0.01;Port=5439;User=postgres;Password=postgres;Database=logistic;CommandTimeout=60"; } }

        public IEnumerable<City> GetCities()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<City>("SELECT * FROM get_cities()");
            }
        }

        public IEnumerable<CarModelCargoType> GetCarModelCargoTypes()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<CarModelCargoType>("SELECT *, get_cargotype_name(m.cargotype_id) as cargotype_name FROM carmodels as m");
            }
        }

        public IEnumerable<CarView> GetCarViews()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<CarView>("SELECT * FROM  cars_view");
            }
        }

        public IEnumerable<CargoType> GetCargoTypes()
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<CargoType>("SELECT * FROM  cargotypes");
            }
        }


        public void BuyCar(int city_id, int carModel_id, DateTime date, string number)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                conn.Execute(@"SELECT * FROM  buy_car(@carModel_id, @number, @city_id, ((@date)::date))",
                    new {  carModel_id = carModel_id, number = number, city_id = city_id, date = date});
            }
        }

        public void SellCar(CarView car, DateTime date)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                conn.Execute(@"SELECT * FROM  sell_car(@car_id, ((@date)::date))",
                    new { car_id = car.id, date = date });
            }
        }

        public IEnumerable<CarViewExpense> FindCarForTransaction(City city_from, City city_to, DateTime date, CargoType cargoType, float weight)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<CarViewExpense>(@"SELECT * FROM cars_view AS cv INNER JOIN find_cars_for_transaction(@cargotype_id, @weight, @city_id_from , @city_id_to, ((@date)::date)) as f ON f.car_id = cv.id",
                    new { cargotype_id = cargoType.id, weight = weight, city_id_from = city_from.id, city_id_to = city_to.id, date = date }).ToList();
              
            }
        }

        public void MakeOrder(CarViewExpense car, City city_from, City city_to, DateTime date, float weight)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                conn.Execute(@"SELECT * FROM  make_transaction(@_car_id, @weight, @_city_id_from, @_city_id_to, ((@date)::date))",
                    new { _car_id = car.id, weight = weight, _city_id_from = city_from.id, _city_id_to = city_to.id, date = date });
            }
        }

        public IEnumerable<TransactionView> GetCarTransactions(Car car)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<TransactionView>(@"SELECT * FROM  transactions_view WHERE car_id = @id", car);
            }
        }

        public IEnumerable<CarStayCoef> GetStayCoefReport(DateTime from, DateTime to)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectStr))
            {
                return conn.Query<CarStayCoef>(@"SELECT * FROM stay_coef_report(@from::date, @to::date) AS r INNER JOIN cars_view AS cv ON cv.id = r.car_id",
                    new { from = from, to = to });
            }
        }
    }
}
