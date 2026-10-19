import * as L from "https://unpkg.com/leaflet@1.9.4/dist/leaflet-src.esm.js";

let map = null;
let gpsLayer = null;
let btsLayer = null;

export function initializeMap(elementId) {
    if (map) return;

    // Center the map on Prague, Czech Republic with a zoom level of 12 << at this point I started to question the extent of intelisense and the amount of code and markups that can be filled automatically, its annoying, that thing managed to fill a whole function for CSV file load, albeit not entirely correct, still usable...
    map = L.map(elementId).setView(
        [50.0755, 14.4378], 12
    );

    //Panes are used to control the order of layers on the map. The zIndex property determines the stacking order of the panes, with higher values appearing above lower values. Without it, last turned on layer will be on top of all other layers.
    map.createPane("gpsPane");
    map.getPane("gpsPane").style.zIndex = 400;

    map.createPane("btsPane");
    map.getPane("btsPane").style.zIndex = 450;
    
    L.tileLayer(
        "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
        {
            maxZoom: 19,
            attribution: '&copy; OpenStreetMap contributors'
        }
    ).addTo(map);

    gpsLayer = L.layerGroup().addTo(map);
    btsLayer = L.layerGroup().addTo(map);
}
export function showGpsRoute(points) {
    if (!map || !gpsLayer) return;

    gpsLayer.clearLayers();

    if (!points || points.length === 0)
        return;

    const coordinates = points.map(p => [
        p.latitude,
        p.longitude
    ]);

    const route = L.polyline(coordinates, {
        pane: "gpsPane",
        color: "#2563eb",
        weight: 4,
        interactive: false
    });

    route.addTo(gpsLayer);

    map.fitBounds(route.getBounds(), {
        padding: [30, 30]
    });
}

export function setGpsVisible(visible) {
    if (!map || !gpsLayer) return;

    if (visible)
        map.addLayer(gpsLayer);
    else
        map.removeLayer(gpsLayer);
}

let btsMarkers = new Map();
let selectedMarkerId = null;

const defaultMarkerStyle = {
    radius: 6,
    color: "#ffffff",
    weight: 2,
    fillColor: "#ea580c",
    fillOpacity: 0.9
};

const selectedMarkerStyle = {
    ...defaultMarkerStyle,
    radius: 10,
    color: "#1d4ed8",
    weight: 3
};

export function showBtsPoints(points, dotNetReference) {
    if (!map || !btsLayer) return;

    btsLayer.clearLayers();
    btsMarkers.clear();
    selectedMarkerId = null;

    if (!points?.length) return;

    const coordinates = [];

    for (const point of points) {
        const lat = point.latitude;
        const lon = point.longitude;

        if (!Number.isFinite(lat) || !Number.isFinite(lon))
            continue;

        const marker = L.circleMarker([lat, lon],
            {pane: "btsPane", ...defaultMarkerStyle});

        marker.on("click", () => {
            selectBtsMarker(point.id);
            dotNetReference.invokeMethodAsync(
                "OnBtsMarkerSelected", point.id);
        });

        marker.addTo(btsLayer);
        btsMarkers.set(point.id, marker);
        coordinates.push([lat, lon]);
    }

    if (coordinates.length > 0) {
        map.fitBounds(L.latLngBounds(coordinates), {
            padding: [30, 30],
            maxZoom: 16
        });
    }
}

export function selectBtsMarker(id) {
    if (selectedMarkerId !== null) {
        const previous = btsMarkers.get(selectedMarkerId);

        if (previous)
            previous.setStyle(defaultMarkerStyle);
    }

    selectedMarkerId = id;

    const marker = btsMarkers.get(id);

    if (marker) {
        marker.setStyle(selectedMarkerStyle);
        marker.bringToFront();
    }
}

export function setBtsVisible(visible) {
    if (!map || !btsLayer) return;

    if (visible)
        map.addLayer(btsLayer);
    else
        map.removeLayer(btsLayer);
}

export function disposeMap() {
    if (map) {
        map.remove();
        map = null;
        gpsLayer = null;
        btsLayer = null;
        btsMarkers.clear();
        selectedMarkerId = null;
    }
}
