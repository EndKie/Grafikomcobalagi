# Natural Island Tops Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Pulihkan bawah Terrain normal pada lima pulau sambil mempertahankan gundukan dan vegetasi di permukaan atas.

**Architecture:** Terrain menjadi satu-satunya geometri pulau. Utility idempoten menghapus child `Natural_Underside`, memenuhi seluruh holes mask, lalu membentuk ulang heightmap dengan noise nol di plateau gameplay.

**Tech Stack:** Unity 6 TerrainData, Mesh, MeshCollider, C# editor tooling

**Spec:** `docs/superpowers/specs/2026-11-22-natural-floating-islands-design.md`

## Global Constraints

- Jangan menjalankan full Math builder.
- Pertahankan 5 block manual, 30 jawaban, satu Water, dan senjata.
- Plateau gameplay tetap datar.
- Auto-builder tetap mati.

## Review Focus

- Seluruh holes sample bernilai `true` agar Terrain utuh.
- Tidak ada `Natural_Underside`, mesh asset, collider, atau material tambahannya.
- Noise tidak mengubah area gate, exit, atau tengah finish.
- Utility aman dijalankan berulang.

---

### Task 1: Surface-Only Contract

**Files:**
- Create: `.superpowers/sdd/2026-11-22-math-mini-islands/check-natural-floating-islands.py`

- [ ] Revisi checker: wajib noise dan plateau protection; larang holes cutout, `Natural_Underside`, dan mesh asset.
- [ ] Jalankan; pastikan RED karena lima underside masih ada.

### Task 2: Terrain-Only Generator

**Files:**
- Modify: `Assets/Editor/BuildMathParkour.cs`

- [ ] Pertahankan radial/noise shaping yang nol di plateau.
- [ ] Isi holes mask dengan `true` untuk mengembalikan Terrain utuh.
- [ ] Hapus generator mesh underside dan semua pemanggilnya.
- [ ] Ubah utility idempoten agar menghapus child underside dari lima root.

### Task 3: Scene Cleanup

**Files:**
- Modify: `Assets/Scenes/GetStarted_Scene.unity`
- Delete: `Assets/Nature/Generated/*Underside.asset`

- [ ] Jalankan utility cleanup tanpa full builder.
- [ ] Verifikasi nol underside, nol mesh asset, Terrain utuh, posisi pulau, compile, dan invariants gameplay.
