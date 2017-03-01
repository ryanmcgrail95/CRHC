﻿using generic;
using generic.mobile;
using System;
using UnityEngine;

public class ScrollMenu : IMenu {
    // TODO: Update scrollFrac if height changes.

    private IMenu menu;
    private float scrollFrac = 0;
    private Color color;

    public ScrollMenu(IMenu menu) {
        this.menu = menu;
    }

    public override void addRow(IRow row) {
        menu.addRow(row);
    }

    public override void draw(float w, float h) {
        // TODO: Pass w/h in via draw, or constructor??
        // might not work right?

        float menuH = menu.getHeight(w), scrollY;
        float heightDiff = menuH - h;

        ITouch iTouch = ServiceLocator.getITouch();

        GUIX.fillRect(new Rect(0, 0, w, h), color);

        if (heightDiff > 0) {
            // Scroll menu.
            scrollFrac -= (iTouch.getDragVector().y / heightDiff);
            scrollFrac = Math.Max(0, Math.Min(scrollFrac, 1));

            scrollY = -scrollFrac * heightDiff;
        }
        else {
            scrollY = 0;
        }

        Vector2 scrollPosition = new Vector2(0, scrollY);
        GUIX.beginClip(new Rect(0, 0, w, h), scrollPosition, Vector2.zero, false);
        //GUIX.beginClip(new Rect(0, scrollY, w, h));
        menu.draw(w, h);
        GUIX.endClip();
    }

    public override float getHeight(float w) {
        return menu.getHeight(w);
    }

    public override void reset() {
        menu.reset();
    }

    public override void setColor(Color color) {
        this.color = color;
    }

    public override void onDispose() {
        menu.Dispose();
        menu = null;
    }
}