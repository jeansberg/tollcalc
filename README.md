# Toll Calculator

## Instructions

Install the .NET 8 SDK.

Run tests from /TollCalculator.Tests with *dotnet test*.

Run application from /TollCalculator with *dotnet run*.

Dates should be entered in the following format: *yyyy-MM-dd hh.mm*.

## Notes

### Implementation
I decided to make this a simple console app for simplicity's sake. A more real world implementation would be a REST API for example. 

### Configuration
I added support for configuring things like toll-free vehicle types and dates. This implementation reads configuration from a static file deployed with the application. Ideally this configuration should support updating without the need for a redeploy or restart. DataAnnotations should be added to validate configuration.  

### Dates
The application and configuration makes some assumptions about dates. All dates are assumed to be in a format appropriate for Sweden, e.g. YYYY-MM-DD. The configuration only contains dates for the year 2013. Instead of hard-coding all dates, a service like https://holidayapi.com/ could be used.

### Testing
The code could be further broken up to make smaller units testable. 

### Security
Since the application does not handle personally identifiable information, security has not been prioritized.

### Other assumptions
Another assumption is that the application only handles passes for one date and vehicle per request. Fees are assumed to always be in whole crowns. If fractions need to be supported, the Decimal type should be used instead.

For this exercise it is also assumed that there is no auditing requirement. In the real-world, an application handling financial transactions should record calculations and results. 