using System;
using System.Collections.Generic;
using System.Linq;
using ClinicApp.Models;

namespace ClinicApp.Managers;

public class AppointmentFilter
{
    private Func<Appointment, bool>? _predicate;

    public AppointmentFilter Add(Func<Appointment, bool> predicate)
    {
        if (_predicate == null)
        {
            _predicate = predicate;
        }
        else
        {
            Func<Appointment, bool> prev = _predicate;
            _predicate = a => prev(a) && predicate(a);
        }

        return this;
    }

    public AppointmentFilter And(Func<Appointment, bool> predicate) =>
        Add(predicate);

    public AppointmentFilter Or(Func<Appointment, bool> predicate)
    {
        if (_predicate == null)
        {
            _predicate = predicate;
        }
        else
        {
            Func<Appointment, bool> prev = _predicate;
            _predicate = a => prev(a) || predicate(a);
        }

        return this;
    }

    public AppointmentFilter Negate()
    {
        if (_predicate != null)
        {
            Func<Appointment, bool> prev = _predicate;
            _predicate = a => !prev(a);
        }

        return this;
    }

    public IEnumerable<Appointment> Apply(IEnumerable<Appointment> source) =>
        _predicate == null ? source : source.Where(_predicate);

    public void Reset()
    {
        _predicate = null;
    }
}