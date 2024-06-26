﻿using LinqTasks.Extensions;
using LinqTasks.Models;

namespace LinqTasks;

public static partial class Tasks
{
    public static IEnumerable<Emp> Emps { get; set; }
    public static IEnumerable<Dept> Depts { get; set; }

    static Tasks()
    {
        Depts = LoadDepts();
        Emps = LoadEmps();
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Job = "Backend programmer";
    /// </summary>
    public static IEnumerable<Emp> Task1()
    {
        return Emps.Where( i => i.Job == "Backend programmer");
    }

    /// <summary>
    ///     SELECT * FROM Emps Job = "Frontend programmer" AND Salary>1000 ORDER BY Ename DESC;
    /// </summary>
    public static IEnumerable<Emp> Task2()
    {
        return Emps.Where( i => i.Job == "Frontend programmer" && i.Salary > 1000).OrderByDescending(i => i.Ename);
    }


    /// <summary>
    ///     SELECT MAX(Salary) FROM Emps;
    /// </summary>
    public static int Task3()
    {
        return Emps.Max(i => i.Salary) ;
    }

    /// <summary>
    ///     SELECT * FROM Emps WHERE Salary=(SELECT MAX(Salary) FROM Emps);
    /// </summary>
    public static IEnumerable<Emp> Task4()
    {
        return Emps.Where(i => i.Salary == Emps.Max(a=> a.Salary));
    }

    /// <summary>
    ///    SELECT ename AS Nazwisko, job AS Praca FROM Emps;
    /// </summary>
    public static IEnumerable<object> Task5()
    {
        return Emps.Select(i=>new{Nazwisko = i.Ename, Praca = i.Job });
    }

    /// <summary>
    ///     SELECT Emps.Ename, Emps.Job, Depts.Dname FROM Emps
    ///     INNER JOIN Depts ON Emps.Deptno=Depts.Deptno
    ///     Rezultat: Złączenie kolekcji Emps i Depts.
    /// </summary>
    public static IEnumerable<object> Task6()
    {
        return Emps.Join(Depts, emp => emp.Deptno, dept => dept.Deptno,
            (emp, dept) => new { emp.Ename, emp.Job, dept.Dname });
    }

    /// <summary>
    ///     SELECT Job AS Praca, COUNT(1) LiczbaPracownikow FROM Emps GROUP BY Job;
    /// </summary>
    public static IEnumerable<object> Task7()
    {
        return Emps.GroupBy(emp => emp.Job).Select(emp => new { Praca = emp.Key, LiczbaPracownikow = emp.Count() });
    }

    /// <summary>
    ///     Zwróć wartość "true" jeśli choć jeden
    ///     z elementów kolekcji pracuje jako "Backend programmer".
    /// </summary>
    public static bool Task8()
    {
        return Emps.Any( i => i.Job == "Backend programmer");
    }

    /// <summary>
    ///     SELECT TOP 1 * FROM Emp WHERE Job="Frontend programmer"
    ///     ORDER BY HireDate DESC;
    /// </summary>
    public static Emp Task9()
    {
        return Emps.Where(i => i.Job == "Frontend programmer").OrderByDescending(i => i.HireDate).FirstOrDefault();
    }

    /// <summary>
    ///     SELECT Ename, Job, Hiredate FROM Emps
    ///     UNION
    ///     SELECT "Brak wartości", null, null;
    /// </summary>
    public static IEnumerable<object> Task10()
    {
        IEnumerable<object> result = Emps
            .Select(emp => new { emp.Ename, emp.Job, emp.HireDate })
            .Union(Emps.Select(emp => new {
                Ename = "Brak wartości",
                Job = string.Empty,
                HireDate = (DateTime?)null
            })).ToList();

        return result;
    }

    /// <summary>
    ///     Wykorzystując LINQ pobierz pracowników podzielony na departamenty pamiętając, że:
    ///     1. Interesują nas tylko departamenty z liczbą pracowników powyżej 1
    ///     2. Chcemy zwrócić listę obiektów o następującej srukturze:
    ///     [
    ///     {name: "RESEARCH", numOfEmployees: 3},
    ///     {name: "SALES", numOfEmployees: 5},
    ///     ...
    ///     ]
    ///     3. Wykorzystaj typy anonimowe
    /// </summary>
    public static IEnumerable<object> Task11()
    {
        IEnumerable<object> result = Emps
            .Join(Depts, emp => emp.Deptno, dept => dept.Deptno, (emp, dept) => new { dept.Dname, emp.Empno })
            .GroupBy(dept => dept.Dname)
            .Select(value => new { name = value.Key, numOfEmployees = value.Count() })
            .Where(value => value.numOfEmployees > 1)
            .ToList();
        return result;
    }

    /// <summary>
    ///     Napisz własną metodę rozszerzeń, która pozwoli skompilować się poniższemu fragmentowi kodu.
    ///     Metodę dodaj do klasy CustomExtensionMethods, która zdefiniowana jest poniżej.
    ///     Metoda powinna zwrócić tylko tych pracowników, którzy mają min. 1 bezpośredniego podwładnego.
    ///     Pracownicy powinny w ramach kolekcji być posortowani po nazwisku (rosnąco) i pensji (malejąco).
    /// </summary>
    public static IEnumerable<Emp> Task12()
    {
        IEnumerable<Emp> result = Emps.GetEmpsWithSubordinates();
        
        return result;
    }

    /// <summary>
    ///     Poniższa metoda powinna zwracać pojedyczną liczbę int.
    ///     Na wejściu przyjmujemy listę liczb całkowitych.
    ///     Spróbuj z pomocą LINQ'a odnaleźć tę liczbę, które występuja w tablicy int'ów nieparzystą liczbę razy.
    ///     Zakładamy, że zawsze będzie jedna taka liczba.
    ///     Np: {1,1,1,1,1,1,10,1,1,1,1} => 10
    /// </summary>
    public static int Task13(int[] arr)
    {
        int result = arr.ToList()
            .GroupBy(value => value.GetHashCode())
            .Select(value => new { Value = value.Key, Times = value.Count() })
            .Where(value => (value.Times % 2) != 0)
            .Select(value => value.Value).FirstOrDefault();

        return result;
    }

    /// <summary>
    ///     Zwróć tylko te departamenty, które mają 5 pracowników lub nie mają pracowników w ogóle.
    ///     Posortuj rezultat po nazwie departament rosnąco.
    /// </summary>
    public static IEnumerable<Dept> Task14()
    {
        IEnumerable<Dept> result = Depts.GroupJoin(Emps, dept => dept.Deptno, emp => emp.Deptno, (dept, emps) => new
        {
            Departament = dept,
            Counter = emps.Count()
        }).Where(record => record.Counter == 5 || record.Counter == 0).Select(record => record.Departament).ToList();

        return result;
    }
    
}
public static class CustomExtensionMethods
{
    public static IEnumerable<Emp> GetEmpsWithSubordinates(this IEnumerable<Emp> emps)
    {
        var result = emps
            .Where(e => emps.Any(sub => sub.Mgr != null && sub.Mgr.Empno == e.Empno))
            .OrderBy(e => e.Ename)
            .ThenByDescending(e => e.Salary);
        return result;
    }
}