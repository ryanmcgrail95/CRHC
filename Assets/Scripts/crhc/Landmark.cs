﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using generic;
using Vuforia;
using Newtonsoft.Json.Linq;
using generic.number;
using generic.rendering;

public class Landmark : CrhcFolder<Experience>, IComparable<Landmark> {
    private Reference<byte[]> dat;
    private Reference<string> xml;

    public IEnumerator loadCoroutine() {
        ILoader loader = ServiceLocator.getILoader();
        dat = loader.getReference<byte[]>(getUrl() + "vuforia/" + getId() + ".dat");
        xml = loader.getReference<string>(getUrl() + "vuforia/" + getId() + ".xml");

        yield return loader.loadCoroutine(dat);
        yield return loader.loadCoroutine(xml);
    }

    protected override IEnumerator tryUnload() {
        foreach (Experience child in this) {
            child.onLandmarkUnload();
        }

        yield return base.tryUnload();
    }

    public override void onDispose() {
        base.onDispose();

        dat.removeOwner();
        xml.removeOwner();
        dat = null;
        xml = null;
    }

    public Reference<byte[]> getDat() {
        return dat;
    }

    public Reference<string> getXML() {
        return xml;
    }

    /*=======================================================**=======================================================*/
    /*=========================================== CONSTRUCTOR/DECONSTRUCTOR ==========================================*/
    /*=======================================================**=======================================================*/
    public Landmark(CrhcItem parent, JObject data) : base(parent, data) {
    }

    public void showMapRoute() {
        MapServer.showRoute(this);
    }

    /*=======================================================**=======================================================*/
    /*============================================== ACCESSORS/MUTATORS ==============================================*/
    /*=======================================================**=======================================================*/
    public Tour getTour() {
        return getParent() as Tour;
    }

    public string getName() {
        return getData<string>("name");
    }

    public int getNumber() {
        return getData<int>("number");
    }

    public string getDescription() {
        return getData<string>("description");
    }

    public string getLongDescription() {
        return getData<string>("longDescription");
    }

    public double getLatitude() {
        return getData<double>("latitude");
    }

    public double getLongitude() {
        return getData<double>("longitude");
    }

    public bool hasAR() {
        return getData<bool>("hasAR");
    }

    public bool hasAddress() {
        return getData<bool>("hasAddress");
    }

    public JArray getAudioClips() {
        return getData<JArray>("audioClips");
    }

    public bool hasAudio() {
        return (getAudioClips().Count > 0);
    }

    public bool isVisible() {
        return getData<bool>("isVisible");
    }

    public override IMenu buildMenu() {
        CoroutineManager.startCoroutine(loadCoroutine());

        Menu menu = new Menu();

        SpaceItem padding = new SpaceItem();

        Row backRow = new Row();
        backRow.addItem(new BackButton(this), 1);
        menu.addRow(backRow);

        Row titleRow = new Row();
        titleRow.setPadding(true, true, false);

        TextItem titleText = new TextItem(getName().ToUpper());
        titleText.setColor(CRHC.COLOR_RED);
        titleText.setFont(CRHC.FONT_SUBTITLE);
        titleRow.addItem(titleText, 1);

        menu.addRow(titleRow);

        Row descRow = new Row();
        descRow.setPadding(true, false, true);

        TextItem descText = new TextItem(getDescription());
        descRow.addItem(descText, 1);

        menu.addRow(descRow);

        int inRow = 0, i = 0;
        Row curRow = null, sourceRow = null;

        Row paddingRow = new Row(30);

        foreach (Experience child in this) {
            child.onLandmarkLoad();

            // Add image.
            if (inRow == 0) {
                inRow++;

                if (i++ > 0) {
                    menu.addRow(paddingRow);
                }

                curRow = new Row();
                curRow.setPadding(true, false, false);
                sourceRow = new Row();
                sourceRow.setPadding(true, false, false);

                menu.addRow(curRow);
                menu.addRow(sourceRow);
            }
            else {
                curRow.addItem(new SpaceItem(), .2f);
                sourceRow.addItem(new SpaceItem(), .2f);
                inRow = 0;
            }

            ImageItem img = new ARButton(child);
            curRow.addItem(img, 1);

            TextItem sourceText = new TextItem(child.getSource());
            sourceText.setFont(CRHC.FONT_SOURCE);
            sourceText.setTextAnchor(TextAnchor.UpperLeft);
            sourceRow.addItem(sourceText, 1);
        }

        if (inRow == 1) {
            curRow.addItem(new SpaceItem(), 1.2f);
            sourceRow.addItem(new SpaceItem(), 1.2f);
        }

        Row row = new Row();
        row.addItem(new TextItem(getLongDescription()), 1);
        menu.addRow(row);
        row.setPadding(true, true, true);

        IMenu scrollMenu = new ScrollMenu(menu);
        IMenu fadeInMenu = new FadeInMenu(scrollMenu, CRHC.SPEED_FADE_IN);

        fadeInMenu.setColor(CRHC.COLOR_BLUE_LIGHT);

        return fadeInMenu;
    }

    public int CompareTo(Landmark other) {
        if (CRHC.LANDMARK_SORTORDER == SortOrder.NAME) {
            return getName().CompareTo(other.getName());
        }
        else {
            return getNumber().CompareTo(other.getNumber());
        }
    }

    public class BackButton : RectItem {
        private CrhcItem owner;
        private Reference<Texture2D> arrowTexture;
        private readonly float PADDING = 8;

        public BackButton(CrhcItem owner) : base(CRHC.COLOR_TRANSPARENT) {
            this.owner = owner;
            arrowTexture = ServiceLocator.getILoader().load<Texture2D>(CachedLoader.SERVER_PATH + "icons/right_icon.png");
        }

        public override void onClick() {
            owner.unload();
        }

        public override bool draw(float w, float h) {
            bool output = base.draw(w, h);
            float arrowW = CRHC.SIZE_BACK_BUTTON.getAs(NumberType.PIXELS);
            float angle = 180;

            //Vector2 pivot = ServiceLocator.getITouch().getTouchPosition(); // new Vector2(PADDING + arrowW / 2, AppRunner.getScreenWidth()-PADDING + arrowW / 2);
            Rect region = new Rect(PADDING, PADDING, arrowW, arrowW);

            float scrH = AppRunner.getScreenHeight();

            //return new Vector2(touchPosition.y, AppRunner.getScreenHeight() - touchPosition.x);
            Vector2 regionCenter = region.center;
            Vector2 pivot;
            if (AppRunner.getIsUpright()) {
                pivot = regionCenter;
            }
            else {
                pivot = new Vector2(scrH - regionCenter.y, scrH + regionCenter.x);
            }

            GUIUtility.RotateAroundPivot(angle, pivot);
            TextureUtility.drawTexture(region, arrowTexture, CRHC.COLOR_GRAY_DARK, AspectType.FIT_IN_REGION);
            GUIUtility.RotateAroundPivot(-angle, pivot);

            return output;
        }

        public override void onDispose() {
            base.onDispose();

            owner = null;

            arrowTexture.removeOwner();
            arrowTexture = null;
        }

        public override float getHeight(float w) {
            return CRHC.SIZE_BACK_BUTTON.getAs(NumberType.PIXELS) + 2 * PADDING;
        }
    }

    private class ARButton : ImageItem {
        private Experience exp;
        private Reference<Texture2D> tex;

        public ARButton(Experience exp) : base(exp.getUrl() + "img.jpg") {
            this.exp = exp;
            tex = ServiceLocator.getILoader().load<Texture2D>(CachedLoader.SERVER_PATH + "icons/ar_icon.png");
        }

        public override void onClick() {
            exp.load();
        }

        public override bool draw(float w, float h) {
            bool output = base.draw(w, h);

            if (tex.isLoaded()) {
                GUIX.Texture(new Rect(0, 0, w / 4, w / 4), tex.getResource());
            }

            return output;
        }

        public override void onDispose() {
            base.onDispose();
            tex.removeOwner();
            tex = null;
            exp = null;
        }
    }
}