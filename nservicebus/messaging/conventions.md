---
title: Conventions
summary: Custom conventions for defining how certain things are detected
tags:
- Unobtrusive
- Convention
component: Core
reviewed: 2019-09-03
related:
 - nservicebus/messaging/messages-events-commands
 - nservicebus/messaging/unobtrusive-mode
---

A *convention* is a way of defining what a certain type is instead of using an interface or an attribute. Using conventions along with avoiding references to NServiceBus assemblies is referred to as *[unobtrusive mode](unobtrusive-mode.md)*. This is ideal for use in cross-platform environments.

Currently conventions exist to identify:

 * [Commands](/nservicebus/messaging/messages-events-commands.md)
 * [Events](/nservicebus/messaging/messages-events-commands.md)
 * [Messages](/nservicebus/messaging/messages-events-commands.md)
 * [Message Property Encryption](/nservicebus/security/property-encryption.md)
 * [Data Bus](/nservicebus/messaging/databus/)
 * [Express messages](/nservicebus/messaging/non-durable-messaging.md)
 * [TimeToBeReceived](/nservicebus/messaging/discard-old-messages.md)

Messages can be defined in a *Portable Class Library* (PCL) and shared across multiple platforms even if the platform does not use NServiceBus for message processing.

snippet: MessageConventions

Note: Note that in .NET, the namespace is optional and hence can be null. If any conventions do partial string checks, for example using `EndsWith` or `StartsWith`, then a null check should be used. So include `.Namespace != null` at the start of the convention. Otherwise a null reference exception will occur during the type scanning.
