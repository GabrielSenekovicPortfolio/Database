using static Database.SkuDatabase;

namespace Database
{
    internal static class DateTimeSorter
    {
        public static ReturnValue HandleOverlap<T>(Dictionary<string, List<Entry>> values,
            T a, T b,
            Func<T, T, bool> comparatorPredicate,
            Action<T> successCallback,
            Action<T> failureCallback) where T : IHasDateRange, ICopyable<T>
        {
            if (OverlapsStartDate(a, b))
            {
                if (b.EndDate() != null && DateTime.Compare(a.EndDate().Value, b.EndDate().Value) <= 0 && comparatorPredicate(a, b))
                { //If the price of a is lower than b, then move b to after a is over
                    b.SetStart(a.EndDate().Value);
                }
                else
                {
                    if (b.EndDate().HasValue && DateTime.Compare(a.EndDate().Value, b.EndDate().Value) > 0)
                    {
                        T newEntry = a.Copy();
                        newEntry.SetStart(b.EndDate().Value);
                        successCallback(newEntry);
                    }
                    a.SetEnd(b.StartDate());
                }
            }

            if (OverlapsEndDate(a, b))
            {
                if (comparatorPredicate(a, b))
                { //If the price of a is lower than b, then shorten b to before a
                    b.SetEnd(a.StartDate());
                }
                else
                {
                    //If the price of a is higher than b, then move a to after b is over,
                    //but split it so there's one copy of the price before b
                    if (b.EndDate().HasValue && DateTime.Compare(a.EndDate().Value, b.EndDate().Value) > 0)
                    {
                        T newEntry = a.Copy();
                        newEntry.SetEnd(b.StartDate());
                        successCallback(newEntry);
                    }
                    a.SetStart(b.EndDate().Value);
                }
            }

            if (!a.EndDate().HasValue)
            {
                if (b.EndDate().HasValue && b.StartDate() == b.EndDate().Value) return ReturnValue.NONE;

                if (DateTime.Compare(a.StartDate(), b.StartDate()) <= 0)
                {
                    if (comparatorPredicate(b, a) && b.EndDate().HasValue)
                    {
                        T newEntry = a.Copy();
                        newEntry.SetStart(b.EndDate().Value);

                        a.SetEnd(b.StartDate());

                        successCallback(newEntry);
                        return ReturnValue.SUCCESS;
                    }
                    else if (comparatorPredicate(b, a) && !b.EndDate().HasValue)
                    {
                        if (DateTime.Compare(a.StartDate(), b.StartDate()) <= 0)
                        {
                            a.SetEnd(b.StartDate());
                            return ReturnValue.SUCCESS;
                        }
                        else
                        {
                            failureCallback(a);
                            return ReturnValue.FAILURE;
                        }
                    }

                    if (comparatorPredicate(a, b) && b.EndDate().HasValue)
                    {
                        failureCallback(b);
                        return ReturnValue.FAILURE;
                    }
                    else if (comparatorPredicate(a, b) && !b.EndDate().HasValue)
                    {
                        a.SetEnd(b.StartDate());
                        return ReturnValue.SUCCESS;
                    }

                    return ReturnValue.FAILURE;
                }
                return ReturnValue.NONE;
            }
            return ReturnValue.SUCCESS;
        }
        public static bool OverlapsStartDate(IHasDateRange a, IHasDateRange b) =>
                a.EndDate().HasValue &&
                DateTime.Compare(a.StartDate(), b.StartDate()) <= 0 &&
                DateTime.Compare(a.EndDate().Value, b.StartDate()) > 0;
        public static bool OverlapsEndDate(IHasDateRange a, IHasDateRange b) =>
                a.EndDate().HasValue && b.EndDate().HasValue &&
                DateTime.Compare(a.StartDate(), b.EndDate().Value) > 0 &&
                DateTime.Compare(a.EndDate().Value, b.EndDate().Value) <= 0;
        public static bool Overlaps(IHasDateRange a, IHasDateRange b) =>
                (a.EndDate().HasValue && DateTime.Compare(a.EndDate().Value, b.StartDate()) < 0) ||
                (b.EndDate().HasValue && DateTime.Compare(a.StartDate(), b.EndDate().Value) > 0);
    }
}