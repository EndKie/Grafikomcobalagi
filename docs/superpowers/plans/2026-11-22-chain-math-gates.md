# Chain Math Gates Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tambahkan course lima tahap soal berantai dengan tiga pintu jawaban dari pulau ketiga ke pulau keempat.

**Architecture:** `ChainMathCourse` mengelola generator soal, nilai berjalan, tahap aktif, teleport benar, dan reset salah. `ChainMathDoor` hanya meneruskan sentuhan pemain dan status jawaban ke controller. Editor utility menambah course ke scene aktif tanpa membangun ulang Math root.

**Tech Stack:** Unity 6, C#, TextMeshPro, CharacterController, editor scene tooling

**Spec:** `docs/superpowers/specs/2026-11-22-chain-math-gates-design.md`

## Global Constraints

- Tepat lima tahap dan tiga pintu per tahap.
- Salah kembali ke awal tahap pertama tanpa mengurangi nyawa global.
- Hasil positif bulat; pembagian selalu habis.
- Pertahankan Math Gate pertama, lima pulau, 5 `Parkour_X_*`, 30 jawaban lama, satu `Water`, dan `SciFiGunLight`.
- Jangan menjalankan full builder/F8 dan jangan menyalakan auto-builder.

## Review Focus

- Trigger non-player tidak boleh mengubah tahap.
- Trigger tahap nonaktif tidak boleh mengubah progres.
- Pembagian tidak pernah menghasilkan pecahan atau nol.
- Teleport harus menonaktifkan `CharacterController` selama perpindahan.
- Utility kedua kali tidak boleh menggandakan course.

---

### Task 1: Runtime Course Logic

**Files:**
- Create: `Assets/Scripts/ChainMathCourse.cs`
- Create: `Assets/Scripts/ChainMathDoor.cs`
- Create: `.superpowers/sdd/2026-11-22-chain-math-gates/check-runtime.py`

**Interfaces:**
- Produces: `ChainMathCourse.SelectDoor(ChainMathDoor door, Transform player)` and `ChainMathDoor.Configure(ChainMathCourse owner, int stageIndex, bool correct)`.

- [x] Tulis checker RED untuk lima tahap, operasi aman, tiga pilihan unik, trigger player-only, teleport controller-safe, dan reset salah.
- [x] Implementasi controller dan door minimum.
- [x] Compile runtime dan jalankan checker GREEN.

### Task 2: Course Geometry Builder

**Files:**
- Modify: `Assets/Editor/BuildMathParkour.cs`
- Create: `.superpowers/sdd/2026-11-22-chain-math-gates/check-scene.py`

**Interfaces:**
- Consumes: komponen runtime Task 1.
- Produces: root scene `Chain_Math_Course` berisi 5 `Chain_Stage_*`, 15 `Chain_Door_*`, layar, label, spawn tahap, start, dan finish.

- [x] Tulis checker scene RED untuk hierarchy, jumlah tahap/pintu, serialized references, dan invariants scene lama.
- [x] Tambah builder khusus idempoten; gunakan platform/papan yang sudah ada di proyek.
- [x] Compile editor dan checker source GREEN.

### Task 3: Scene Migration and Verification

**Files:**
- Modify: `Assets/Scenes/GetStarted_Scene.unity`

**Interfaces:**
- Consumes: builder Task 2.

- [x] Jalankan bootstrap sekali pakai yang hanya menambah `Chain_Math_Course`.
- [x] Pastikan bootstrap menghapus dirinya.
- [x] Verifikasi 5 tahap, 15 pintu, fileID unik, compile Unity, invariants lama, dan tidak ada error log baru.
