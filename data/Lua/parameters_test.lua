-- parameters_test.lua by anotak
-- this is just a test for now

UI.AddParameter("add_a", "this will get added to the next one", 2)
UI.AddParameter("add_b", "yea, this one", 3)
UI.AddParameter("some_other", "this one will be ignored", "yep")
UI.AddParameter("other_other", "this one doesn't even have a default value")

parameters = UI.AskForParameters()

UI.LogLine(parameters.add_a + parameters.add_b)