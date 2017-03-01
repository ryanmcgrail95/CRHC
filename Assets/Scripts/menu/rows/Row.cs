<<<<<<< HEAD
﻿using generic.number;
using System;
=======
﻿using System;
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737
using System.Collections.Generic;
using UnityEngine;

public class Row : IRow {
    private float height;

    public Row() {
        height = 0;
    }

    public Row(float height) {
        this.height = height;
    }

    private struct Pair {
        public IItem item;
        public float priority;

        public Pair(IItem item, float priority) {
            this.item = item;
            this.priority = priority;
        }
    }

    private IList<Pair> items = new List<Pair>();
<<<<<<< HEAD
    private Color color = CRHC.COLOR_TRANSPARENT;
=======
    private Color color = Crch.COLOR_TRANSPARENT;
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737
    private bool doXPad, doYPadTop, doYPadBottom;

    public void addItem(IItem item, float priority) {
        items.Add(new Pair(item, priority));
    }

    public void setColor(Color color) {
        this.color = color;
    }

    public override bool draw(float w) {
<<<<<<< HEAD
        float x = 0, h = getPixelHeight(w);
=======
        float x = 0, h = getHeight(w);
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737

        bool alreadyClicked = false;

        Rect position = new Rect(0, 0, w, h + 1);
        GUIX.fillRect(position, color);

        if (doXPad) {
<<<<<<< HEAD
            float pad = CRHC.PADDING_H.getAs(NumberType.PIXELS);
=======
            float pad = w * Crch.H_PADDING;
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737

            w -= 2 * pad;
            x += pad;
        }

        float y = 0;
        if (doYPadTop) {
<<<<<<< HEAD
            y += CRHC.PADDING_V.getAs(NumberType.PIXELS);
            h -= CRHC.PADDING_V.getAs(NumberType.PIXELS);
        }
        if (doYPadBottom) {
            h -= CRHC.PADDING_V.getAs(NumberType.PIXELS);
=======
            y += Crch.V_PADDING;
            h -= Crch.V_PADDING;
        }
        if (doYPadBottom) {
            h -= Crch.V_PADDING;
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737
        }

        IItem item;
        float totalPriority = 0, sw;
        foreach (Pair pair in items) {
            totalPriority += pair.priority;
        }

        foreach (Pair pair in items) {
            item = pair.item;
            sw = pair.priority / totalPriority * w;

            GUIX.beginClip(new Rect(x, y, sw, h));
            alreadyClicked = item.draw(sw, h) || alreadyClicked;
            GUIX.endClip();
            x += sw;
        }

        if (!alreadyClicked) {
            return GUIX.isMouseInsideRect(position);
        }
        else {
            return false;
        }
    }

<<<<<<< HEAD
    public override float getPixelHeight(float w) {
=======
    public override float getHeight(float w) {
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737
        float totalPriority = 0, sw;
        foreach (Pair pair in items) {
            totalPriority += pair.priority;
        }

        if (doXPad) {
<<<<<<< HEAD
            float pad = CRHC.PADDING_H.getAs(NumberType.PIXELS);
=======
            float pad = w * Crch.H_PADDING;
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737
            w -= 2 * pad;
        }

        float h = 0;

        IItem item;
        foreach (Pair pair in items) {
            item = pair.item;
            sw = pair.priority / totalPriority * w;

            h = Math.Max(h, item.getHeight(sw));
        }

        if (doYPadTop) {
<<<<<<< HEAD
            h += CRHC.PADDING_V.getAs(NumberType.PIXELS);
        }
        if (doYPadBottom) {
            h += CRHC.PADDING_V.getAs(NumberType.PIXELS);
=======
            h += Crch.V_PADDING;
        }
        if (doYPadBottom) {
            h += Crch.V_PADDING;
>>>>>>> 7d8058b78fc3336b912526ca3bdad1b73a459737
        }

        return Math.Max(h, height);
    }

    public void setPadding(bool enableX, bool enableYTop, bool enableYBottom) {
        doXPad = enableX;
        doYPadTop = enableYTop;
        doYPadBottom = enableYBottom;
    }

    public override void onDispose() {
        for (int i = 0; i < items.Count; i++) {
            items[i].item.Dispose();
        }
        items.Clear();
        items = null;
    }
}