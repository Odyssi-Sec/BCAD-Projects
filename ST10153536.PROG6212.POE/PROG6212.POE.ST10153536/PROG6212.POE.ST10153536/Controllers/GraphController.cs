using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using PROG6212.POE.ST10153536.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

public class GraphController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GraphController> _logger; 


    public GraphController(ApplicationDbContext context, ILogger<GraphController> logger) 
    {
        _context = context;
        _logger = logger;
    }
     public IActionResult LineGraph()
     {
        try
        {
            _logger.LogInformation("Attempting to fetch data and prepare GraphData...");

            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var graphData = new GraphData
            {
                Labels = GetWeekLabels(),
                HoursSpent = GetHoursData("HoursSpent", userId),
                CurrentWeekSelfStudyRemain = GetHoursData("CurrentWeekSelfStudyRemain", userId),
                SelfStudyHours = GetHoursData("SelfStudyHours", userId)
            };

            _logger.LogInformation("Successfully fetched data and prepared GraphData. Rendering the view...");

            return View(graphData);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred in the GraphController: {ex.Message}");
            return View(new GraphData());
        }
     }

    private List<string> GetWeekLabels()
    {
        Console.WriteLine("Fetching week labels...");

        var startDates = _context.Modules.OrderBy(m => m.StartDate).Select(m => m.StartDate).ToList();

        Console.WriteLine("Successfully fetched week labels.");

        return startDates.Select((startDate, index) => $"Week {index + 1}").ToList();
    }

    private List<double?> GetHoursData(string fieldName, int userId)
    {
        var modules = _context.Modules
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.StartDate)
            .ToList();

        var hoursData = new List<double?>();

        foreach (var module in modules)
        {
            var value = GetPropertyValue<double?>(module, fieldName);
            hoursData.Add(value);
        }

        for (int i = 0; i < hoursData.Count; i++)
        {
            if (hoursData[i] == null)
            {
                hoursData[i] = 0;
            }
        }

        return hoursData;
    }


    public static T GetPropertyValue<T>(object obj, string propertyName)
    {
        if (obj == null)
        {
            return default(T);
        }

        var propertyInfo = obj.GetType().GetProperty(propertyName);
        if (propertyInfo != null)
        {
            var value = propertyInfo.GetValue(obj);

            if (typeof(T) == typeof(double) && value is int intValue)
            {
                return (T)(object)(double)intValue;
            }

            if (typeof(T) == typeof(double?) && value is int intValueForNullable)
            {
                return (T)(object)(double?)intValueForNullable; 
            }

            return (T)value;
        }

        return default(T);
    }
}
