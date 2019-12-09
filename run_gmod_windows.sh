#!/bin/bash

# Run your app in the background
powershell -Command './gmod/srcds_win64.exe -console -systemtest -game "garrysmod" +exec "server.cfg" +gamemode sandbox +map gm_construct +maxplayers 16 &'

# Store it's Process ID
bg_pid=$!

# sleep for X seconds
sleep 120

# Kill the python process
kill $bg_pid

# Optionally exit true to prevent travis seeing this as an error
exit 0