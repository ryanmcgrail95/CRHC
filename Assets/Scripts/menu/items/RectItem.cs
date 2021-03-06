﻿using UnityEngine;

public class RectItem : IItem {
    private Color color;

    public RectItem(Color color) {
        this.color = color;
    }

    public override bool draw(float w, float h) {
        Rect rect = new Rect(0, 0, w, h);

        GUIX.fillRect(rect, color);
        if (GUIX.didTapInsideRect(rect)) {
            onClick();
            return true;
        }
        else {
            return false;
        }
    }

    protected override float calcPixelHeight(float w) {
        return 30;
    }
}
