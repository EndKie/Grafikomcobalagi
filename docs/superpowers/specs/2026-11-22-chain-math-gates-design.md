# Chain Math Gates Design

## Intent

Tambahkan course matematika berantai dari `Math_High_Island` menuju `Math_Upper_Arc_Island`. Pemain menyelesaikan lima soal melalui tiga pintu jawaban pada setiap tahap.

## Flow

- Course dimulai di tepi `Math_High_Island` dan selesai di `Math_Upper_Arc_Island`.
- Tahap pertama memberi angka awal acak positif.
- Satu layar pada setiap tahap menampilkan satu operasi lengkap, misalnya `8 × 3 = ?`.
- Tiga pintu di bawah layar menampilkan angka jawaban; tepat satu benar.
- Memilih jawaban benar memindahkan pemain ke awal tahap berikutnya.
- Hasil benar menjadi angka awal soal berikutnya.
- Setelah tahap kelima benar, pemain dipindahkan ke area aman pulau keempat.
- Memilih jawaban salah langsung memindahkan pemain ke awal tahap pertama, mengatur ulang progres, dan membuat rantai soal baru.
- Gagal tidak mengurangi nyawa global dan tidak memicu Game Over.

## Questions

- Tepat lima tahap.
- Operasi memakai `+`, `−`, `×`, dan `÷`.
- Semua hasil positif dan bulat.
- Pembagian selalu habis tanpa pecahan.
- Dua jawaban pengecoh harus berbeda dari jawaban benar dan satu sama lain.
- Nilai dijaga dalam rentang wajar agar terbaca cepat.

## Presentation

- Setiap tahap memiliki satu platform aman, satu layar soal, dan tiga pintu/lorong sejajar.
- Label angka ditempatkan di atas atau pada pintu.
- Hanya tahap aktif yang menerima pilihan; menyentuh trigger tahap lain tidak mengubah progres.
- Pintu benar dan salah tidak diberi warna pembeda sebelum dipilih.
- Course mengikuti arah diagonal dan kenaikan elevasi dari pulau ketiga ke pulau keempat.

## Implementation Approach

- Pilihan: controller course khusus dan trigger pintu khusus. Controller menyimpan tahap, nilai berjalan, soal, reset, dan teleport.
- Ditolak: memakai `MathQuestionStage`, karena sistem lama menjatuhkan platform dan tidak mendukung reset seluruh rantai.
- Ditolak: mengubah sistem respawn global, karena jawaban salah tidak boleh mengurangi nyawa atau mengganggu checkpoint lain.
- Scene ditambah lewat utility idempoten yang hanya menyentuh root course baru. Full builder/F8 tidak dijalankan.

## Constraints

- Pertahankan Math Gate pertama dan seluruh perilakunya.
- Pertahankan lima pulau, 5 `Parkour_X_*`, 30 jawaban lama, satu `Water`, dan `SciFiGunLight`.
- Jangan menambah checkpoint global, logika finish, atau mengubah nyawa pemain.
- Auto-builder tetap mati.
