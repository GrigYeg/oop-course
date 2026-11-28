using System;
using System.Collections.Generic;
using System.Linq;
using ClinicApp.Models;

namespace ClinicApp.Managers;

public class AppointmentPipeline
{
    private readonly AppointmentFilter _filter;
    private readonly AppointmentProcessor _processor;

    public AppointmentPipeline()
    {
        _filter = new AppointmentFilter();
        _processor = new AppointmentProcessor();
    }

    public AppointmentPipeline Filter(Func<Appointment, bool> predicate)
    {
        _filter.Add(predicate);
        return this;
    }

    public AppointmentPipeline Then(Action<Appointment> action)
    {
        _processor.Run(action);
        return this;
    }

    public int Execute(IEnumerable<Appointment> source)
    {
        Appointment[] filtered = _filter.Apply(source).ToArray();
        _processor.Execute(filtered);
        return filtered.Length;
    }

    public void Reset()
    {
        _filter.Reset();
        _processor.Clear();
    }
}