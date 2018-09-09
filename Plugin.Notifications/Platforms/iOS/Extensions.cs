﻿using System;
using CoreLocation;
using Foundation;
using UserNotifications;


namespace Plugin.Notifications
{
    public static class iOSExtensions
    {
        public static DateParts FromNative(this NSDateComponents native)
        {
            var parts = new DateParts();
            if (native.Date != null)
            {
            }
            return parts;
        }


        public static INotificationTrigger FromNative(this UNNotificationTrigger native)
        {
            if (native is UNLocationNotificationTrigger cast1)
                return new LocationNotificationTrigger(cast1.Region.Radius, cast1.Region.Center.Latitude, cast1.Region.Center.Longitude, cast1.Repeats);

            if (native is UNCalendarNotificationTrigger cast2)
                return new CalendarNotificationTrigger(cast2.DateComponents.FromNative(), cast2.Repeats); // TODO

            if (native is UNTimeIntervalNotificationTrigger cast3)
                return new TimeIntervalNotificationTrigger(TimeSpan.FromSeconds(cast3.TimeInterval), cast3.Repeats);

            throw new NotSupportedException("Invalid notification trigger type");
        }


        public static Notification FromNative(this UNNotificationRequest native) => new Notification
        {
            Id = native.Identifier,
            Title = native.Content.Title,
            Message = native.Content.Body,
            Sound = native.Content.Sound?.ToString(),
            Payload = "",
            Trigger = native.Trigger.FromNative()
            //ScheduledDate = date,
            //Metadata = native.Content.UserInfo.FromNsDictionary()
        };

        public static UNNotificationTrigger ToNative(this INotificationTrigger trigger)
        {
            if (trigger is CalendarNotificationTrigger cast1)
                return cast1.ToNative();

            if (trigger is TimeIntervalNotificationTrigger cast2)
                return cast2.ToNative();

            if (trigger is LocationNotificationTrigger cast3)
                return cast3.ToNative();

            throw new NotSupportedException("Invalid notification trigger type");
        }


        public static NSDateComponents ToNative(this DateParts dateParts)
        {
            var native = new NSDateComponents();
            if (dateParts.Date != null)
            {
                var dt = dateParts.Date.Value;

                native.Year = dt.Year;
                native.Month = dt.Month;
                native.Day = dt.Day;
            }

            if (dateParts.TimeOfDay != null)
            {
                var td = dateParts.TimeOfDay.Value;
                native.Hour = td.Hours;
                native.Minute = td.Minutes;
                native.Second = td.Seconds;
            }

            if (dateParts.DayOfWeek != null)
                native.Weekday = (int) dateParts.DayOfWeek;

            //if (dateParts.WeekOfYear)
            //dateParts.DayOfYear

            return native;
        }


        public static UNCalendarNotificationTrigger ToNative(this CalendarNotificationTrigger trigger)
            => UNCalendarNotificationTrigger.CreateTrigger(trigger.DateParts.ToNative(), trigger.Repeats);


        public static UNLocationNotificationTrigger ToNative(this LocationNotificationTrigger trigger, string identifier) => UNLocationNotificationTrigger
            .CreateTrigger(
                new CLCircularRegion(
                    new CLLocationCoordinate2D(
                        trigger.GpsLatitude,
                        trigger.GpsLongitude
                    ),
                    trigger.RadiusInMeters,
                    identifier
                )
                {
                    NotifyOnEntry = true
                },
                trigger.Repeats
            );


        public static UNTimeIntervalNotificationTrigger ToNative(this TimeIntervalNotificationTrigger trigger)
            => UNTimeIntervalNotificationTrigger.CreateTrigger(trigger.Interval.TotalSeconds, trigger.Repeats);
    }
}