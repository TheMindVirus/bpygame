# Window -> Toggle System Console

import bpy

import pip
pip.main(["install", "pygame", "--user"])

import pygame

from pygame import joystick

joysticks = None

def refresh():
    global joysticks
    joystick.quit()
    joystick.init()
    c = joystick.get_count()
    joysticks = [joystick.Joystick(x) for x in range(c)]

refresh()
print(joysticks[0])

import time

# Uncomment if no instances are running but it thinks there's one
# bpy.context.scene.collection["bpy_running"] = 0

def daemon():
    global joysticks
    try:
        bpy.context.scene.collection["bpy_running"] += 1
    except:
        bpy.context.scene.collection["bpy_running"] = 0
    running = True
    print(bpy.context.scene.collection["bpy_running"])
    while running:
        try:
            refresh()
            x1 = joysticks[0].get_axis(0)
            y1 = joysticks[0].get_axis(1)
            x2 = joysticks[0].get_axis(5)
            y2 = joysticks[0].get_axis(4)
            a = joysticks[0].get_button(0)
            b = joysticks[0].get_button(1)
            print(x1)
            time.sleep(1)
            if bpy.context.scene.collection["bpy_running"] > 1:
                bpy.context.scene.collection["bpy_running"] -= 1
                running = False
        except:
            raise
            pass

import threading

thread = threading.Thread(target = daemon)
thread.start()

# Control-S and Control-C in System Console to Stop Thread
# If it doesn't stop, save and restart (ideally save before running)