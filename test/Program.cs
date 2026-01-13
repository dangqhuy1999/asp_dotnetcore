using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// app.MapGet("/", () => "Hello World!"); // Visible middleware component

// basic routing 
// the middleware component is going to use it to handle all requests
// http requests body is empty in this case, use for PUT/POST with body content
app.Run(async (HttpContext context) =>
{


    if (context.Request.Path.StartsWithSegments("/"))
    {
        if (context.Request.Method == "GET")
        {
            context.Response.Headers["Content-Type"] = "text/html";

            // context.Request.Method fo all urls
            await context.Response.WriteAsync($"The method is: {context.Request.Method}<br/>");
            await context.Response.WriteAsync($"The URL is: {context.Request.Path}<br/>");

            await context.Response.WriteAsync($"<b>Header: </b>");
            await context.Response.WriteAsync($"<ul>");
            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"<li><b>{key}</b> : {context.Request.Headers[key]}<br/></li>");
            }
            await context.Response.WriteAsync($"</ul>");
        }

    }
    else if (context.Request.Path.StartsWithSegments("/employees"))
    {
        if (context.Request.Method == "GET")
        {
            List<Employee> listEmployees = EmployeesRepository.GetEmployees();
            foreach (var emp in listEmployees)
            {
                await context.Response.WriteAsync($"Id: {emp.Id}, Name: {emp.Name}, Position: {emp.Position}, Salary: {emp.Salary}\r\n");
            }

            context.Response.StatusCode = 200; // successfull to get
        }
        else if (context.Request.Method == "POST")
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            EmployeesRepository.AddEmployee(employee);
            context.Response.StatusCode = 201; // successfull to create
        }

        else if (context.Request.Method == "PUT")
        {

            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            var check = EmployeesRepository.UpdateEmployee(employee);
            if (check)
            {
                context.Response.StatusCode = 204; // 204 must be exlude with other Context.Response.WriteAsync notification
                // await context.Response.WriteAsync($"Employee with Id: {employee?.Id} updated successfully.\r\n");

                return;
            }
            else
            {
                await context.Response.WriteAsync($"Employee with Id: {employee?.Id} not found.\r\n");
            }
            
        }

        else if (context.Request.Method == "DELETE")
        {
            if (context.Request.Query.ContainsKey("id"))
            { // looking for the 'id' of employee
                var id = context.Request.Query["id"]; // id is string value
                if (int.TryParse(id, out int employeeId))
                {
                    if (context.Request.Headers["Authorization"] == "Franklin")
                    {
                        var result = EmployeesRepository.DeleteEmployee(employeeId);
                        if (result)
                        {
                            await context.Response.WriteAsync($"Employee {employeeId} is deleted succesfully!");
                        }
                        else
                        {
                            await context.Response.WriteAsync($"Employee not found!");
                        }
                    }
                    else
                    {
                        await context.Response.WriteAsync("You are not authorized to delete.");
                    }
                }
            }
        }

    }

    // in browser, you can see the result
    // with http://localhost:5145/ or http://localhost:5145/test and so on.
    // although there is no visible middleware component.
});

app.Run();


public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public double Salary { get; set; }

    public Employee(int id, string name, string position, double salary)
    {
        Id = id;
        Name = name;
        Position = position;
        Salary = salary;
    }
}

// I never want anyone to create an object of this class using new.
// using like a temporarily database table
static class EmployeesRepository
{
    private static List<Employee> employees = new List<Employee>
    {
        new Employee(1, "Alice Johnson", "Software Engineer", 90000),
        new Employee(2, "Bob Smith", "Project Manager", 105000),
        new Employee(3, "Charlie Brown", "QA Analyst", 75000),
        new Employee(4, "Diana Prince", "UX Designer", 85000)
    };
    public static List<Employee> GetEmployees() => employees;
    public static void AddEmployee(Employee? emp)
    {
        if (emp is not null)
        {
            employees.Add(emp);
        }

    }
    public static bool UpdateEmployee(Employee? employee)
    {
        if (employee is not null)
        {
            var emp = employees.FirstOrDefault(e => e.Id == employee.Id); // Has Value/Null to prevent not found element exception
            if (emp is not null)
            {
                emp.Name = employee.Name;
                emp.Position = employee.Position;
                emp.Salary = employee.Salary;
                return true;
            }
        }
        return false;
    }

    public static bool DeleteEmployee(int id)
    {
        var employee = employees.FirstOrDefault(x => x.Id == id);
        if (employee is not null)
        {
            employees.Remove(employee);
            return true;
        }
        return false;
    }
}
// I already Click yes to install certificates for http requests.
// https://www.youtube.com/watch?v=2c99pIfZq1s