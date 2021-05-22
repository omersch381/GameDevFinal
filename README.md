This is my game development assignment #3, which is a race between 3 NPCs: Claire, Mouse and Racer.
The differences between these 3 participants are: their appearance and their speed.
The difference in their speed is like the following:
	Claire: speed is constant and stays 5f.
	Mouse: speed goes up an down from 1f to 10f (Ex: 1->2->...->10->9->8->...->1->2->...)
	Racer: speed is being generated randomly within the range: [1f, 10f].

The speed change interval is one second, but it can be changed by the user preference.
There is a common board (UI element) which shows their score (score == number of times they reached the target).

Every time they reach the target, the target moves to another location, out of 6 constant locations.
