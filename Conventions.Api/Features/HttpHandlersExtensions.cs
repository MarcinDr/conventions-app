using Conventions.Api.Features.AttendTalk;
using Conventions.Api.Features.CreateEvent;
using Conventions.Api.Features.CreateTalk;
using Conventions.Api.Features.GetEvents;
using Conventions.Api.Features.GetEventsByAccount;
using Conventions.Api.Features.GetTalkById;
using Conventions.Api.Features.GetTalksByEvent;
using Conventions.Api.Features.GetVenues;

namespace Conventions.Api.Features;

public static class HttpHandlersExtensions
{
	public static WebApplication MapHttpHandlers(this WebApplication app)
	{
		return app
			.MapCreateEventHandler()
			.MapCreateTalkHandler()
			.MapGetTalksByEventHandler()
			.MapGetEventsHandler()
			.MapGetTalksByIdHandler()
			.MapGetVenuesHandler()
			.MapAttendTalkHandler()
			.MapGetEventsByAccountHandler();
	}
}