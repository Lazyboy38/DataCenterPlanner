# DataCenterPlanner

Rack and network planning tool for the game Data Center.

## Features

- Rack planning by IOPS
- Multiple server categories
- Rack usage visualization
- Network switch calculation
- QSFP / RJ45 SFP pack estimation

## Current Version

v0.3.0-alpha

## Disclaimer:

This is mostly a Vibe Coding project, i know i know, its not from you if AI helped you. 
I understand this standpoint but this is the first project i think made sense for me and thats was very fun planning out. And i do plan to continue working on this for quite some time.
If you dont like Vibe COding projects, don't use it. i dont make money from it or anything else. I just found it difficult to plan sometimes in this game. and its a lot of fun!

## Usage:

DataCenterPlanner calculates a complete rack and basic network layout based on the required IOPS per server category.

### 1. Select a planning mode

Two planning strategies are currently available:

"Marius Mode" (name will be changed eventually, as this was my first thought since i love visually appealing racks instead of packed opnes)
Distributes racks in a structured way and keeps racks visually balanced where possible.
Minimum Racks
Packs hardware into as few racks as possible.

### 2. Enter required IOPS per server family
You can define target IOPS separately for:

SystemX
RISC
Mainframe
GPU

The planner automatically selects:

12k servers (12,000 IOPS / 7U)
5k servers (5,000 IOPS / 3U)

to reach or slightly exceed the requested target.

### 3. Optional: Enable redundancy

When Plan with redundancy is enabled:

each server is calculated with two cable connections
this doubles required switch ports
additional rack switches may be required automatically

Hardware assumptions currently used:
Rack capacity
1 Rack = 47U

Server sizes
12k Server = 7U
5k Server = 3U

Switch size
every switch uses 1U

Rack logic
Every rack always contains at least one rack switch
Exactly one global main rack receives an additional switch
This global main rack acts as the central uplink point to the customer cabinet

### Network logic

Rack switches:
Rack switches are currently based on:

4x QSFP+ + 16x SFP+/SFP28
32x QSFP+

They are used both for:

local server connections
uplink to the main switch
Port logic

Without redundancy:

1 server = 1 required switch port

With redundancy:

1 server = 2 required switch ports

Rack switch count is calculated automatically from required ports.

SFP calculation
QSFP modules

QSFP modules are calculated for:

rack switch → main switch
main switch → customer rack
RJ45 SFP modules

RJ45 SFP modules are calculated for all server cable connections.

All SFP modules are converted into 5-pack purchase units.

### Current simplifications

This version intentionally uses simplified assumptions:

cable lengths are not considered
exact port assignment inside individual switches is simplified
all racks currently use the same rack-switch strategy
Switch modes for combining ports to achive higher transfer speeds are not considered

The goal is fast practical planning inside the game. And after all this is the first Version, currently only tested by me

## Known issues:

Scrolling only works when not hovering over the result Cards
Some Summary Cards are sometimes not sized evenly on other screens

Please feel free to comment for features you might want or simply use github to contribute.

