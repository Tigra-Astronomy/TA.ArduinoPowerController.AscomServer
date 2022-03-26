# Device Interface project

It is best practice to place the device communications and interfacing code in its own separate project. One reason for doing so is so that this "business logic" code can be unit tested in isolation.

This will become particularly important in projects that contain more than one driver class. The driver classes can then share the common device interface code.