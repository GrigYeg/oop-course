using System;
using ClinicApp.Interfaces;
using ClinicApp.Models;

namespace ClinicApp.Managers
{
    public class BillingManager
    {
        private readonly AppointmentManager _appointmentManager;

        public BillingManager(AppointmentManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
        }

        private IPayable[] FilterUnpaid(Appointment[] source)
        {
            int count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] is IPayable payable && !payable.IsPaid)
                {
                     if (source[i] is ICancellable cancellable && cancellable.IsCancelled)
                     {
                         continue;
                     }
                     count++;
                }
            }

            IPayable[] result = new IPayable[count];
            int index = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] is IPayable payable && !payable.IsPaid)
                {
                    if (source[i] is ICancellable cancellable && cancellable.IsCancelled)
                    {
                        continue;
                    }
                    result[index++] = payable;
                }
            }
            return result;
        }

        public IPayable[] GetAllUnpaid()
        {
            return FilterUnpaid(_appointmentManager.GetAll());
        }

        public IPayable[] GetUnpaidByPatient(int patientId)
        {
            return FilterUnpaid(_appointmentManager.GetByPatient(patientId));
        }

        public decimal GetTotalDebt()
        {
            IPayable[] unpaid = GetAllUnpaid();
            decimal total = 0m;
            for (int i = 0; i < unpaid.Length; i++)
            {
                total += unpaid[i].GetCost();
            }
            return total;
        }

        public decimal GetPatientDebt(int patientId)
        {
            IPayable[] unpaid = GetUnpaidByPatient(patientId);
            decimal total = 0m;
            for (int i = 0; i < unpaid.Length; i++)
            {
                total += unpaid[i].GetCost();
            }
            return total;
        }

        public bool PayAppointment(int appointmentId)
        {
            Appointment[] all = _appointmentManager.GetAll();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].Id == appointmentId)
                {
                    if (all[i] is IPayable payable)
                    {
                        if (payable.IsPaid) return false;
                        payable.MarkPaid();
                        return payable.IsPaid;
                    }
                    return false;
                }
            }
            return false;
        }

        public void DisplayUnpaid(IPayable[] items)
        {
            if (items.Length == 0)
            {
                Console.WriteLine("Немає неоплачених записів.");
                return;
            }

            for (int i = 0; i < items.Length; i++)
            {
                string itemInfo;
                if (items[i] is Appointment app)
                {
                    itemInfo = app.ToString();
                }
                else
                {
                    itemInfo = $"Елемент #{i + 1}";
                }
                
                Console.WriteLine($"{itemInfo} | Сума: {items[i].GetCost().ToString("F2")} грн");
            }
        }
    }
}