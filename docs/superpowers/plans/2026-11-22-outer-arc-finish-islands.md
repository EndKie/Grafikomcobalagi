# Outer Arc and Finish Islands Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Tambah pulau transisi tinggi pada busur luar dan pulau finish besar di tengah tanpa mengubah gameplay atau edit manual scene.

**Architecture:** Tambah dua `TerrainData` unik dan dua root terrain langsung ke scene. Generator menerima definisi sama untuk rebuild manual masa depan, tetapi tidak dijalankan selama implementasi agar penghapusan dan posisi manual tetap utuh.

**Tech Stack:** Unity 6, C#, Unity YAML scene, TerrainData

**Spec:** `docs/superpowers/specs/2026-11-22-outer-arc-finish-islands-design.md`

## Global Constraints

- `Math_Upper_Arc_Island` berpusat `(-90, 59, 90)` dan melayang.
- `Final_Center_Island` berpusat `(0, 68, 80)`, lebih besar, dan melayang.
- Tidak ada akses, checkpoint, bridge, platform, atau logika finish.
- Jangan rebuild seluruh Math root; pertahankan 5 `Parkour_X_*` hasil edit manual.
- Auto-builder tetap mati.

## Review Focus

- TerrainData kedua pulau harus unik.
- Dekorasi tidak memenuhi tengah pulau finish.
- Scene tetap punya 30 jawaban, satu Water, dan senjata.
- Tidak ada duplicate Unity fileID.
- Kedua terrain tidak menyentuh air.

---

### Task 1: Geometry Contract

**Files:**
- Create: `.superpowers/sdd/2026-11-22-math-mini-islands/check-outer-arc-finish.py`

- [ ] Buat checker posisi, ukuran, elevasi, TerrainData unik, dekorasi, dan invariants scene.
- [ ] Jalankan checker; pastikan gagal karena dua pulau belum ada.

### Task 2: Terrain Assets and Generator Definition

**Files:**
- Modify: `Assets/Editor/BuildMathParkour.cs`
- Create: `Assets/Nature/Generated/MathUpperArcIslandTerrain.asset`
- Create: `Assets/Nature/Generated/FinalCenterIslandTerrain.asset`

- [ ] Tambah definisi dua pulau dan dekorasi tepi pada generator.
- [ ] Naikkan marker ke `Build_v20`, tetapi jangan menjalankan builder.
- [ ] Buat TerrainData unik lewat editor utility yang tidak mengubah Math root.

### Task 3: Scene Integration

**Files:**
- Modify: `Assets/Scenes/GetStarted_Scene.unity`

- [ ] Tambah dua root terrain dan dekorasinya tanpa menyentuh objek lain.
- [ ] Jalankan checker, validasi fileID, compile editor/runtime.
- [ ] Pastikan `Parkour_X_*` tetap 5 dan auto-builder tetap mati.
