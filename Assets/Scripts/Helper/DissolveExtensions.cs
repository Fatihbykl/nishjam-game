using DG.Tweening;
using UnityEngine;

namespace Helper
{
    public static class DissolveExtensions
    {
        // Shader'daki property ismini buraya yaz. Genelde "_Cutoff" veya "_DissolveAmount" olur.
        private static readonly int CutoffProperty = Shader.PropertyToID("_Cutoff");

        /// <summary>
        /// Bir Renderer üzerindeki materyalin Cutoff değerini değiştirir.
        /// </summary>
        /// <param name="renderer">Hedef Renderer</param>
        /// <param name="targetValue">Hedef değer (0: Görünür, 1: Yok olmuş)</param>
        /// <param name="duration">Süre</param>
        public static Tween DODissolve(this Renderer renderer, float targetValue, float duration)
        {
            // Renderer üzerindeki tüm materyalleri etkilemesi için döngü (örn: karakterin hem kafası hem gözlüğü varsa)
            Sequence seq = DOTween.Sequence();
        
            foreach (var mat in renderer.materials)
            {
                // DOTween'in kendi malzeme değiştirme metodu:
                seq.Join(mat.DOFloat(targetValue, CutoffProperty, duration));
            }

            return seq;
        }

        /// <summary>
        /// Nesneyi yok eder (Dissolve Out)
        /// </summary>
        public static Tween DODissolveOut(this Renderer renderer, float duration = 1f)
        {
            // 0'dan 1'e gider (Shader ayarına göre değişebilir, genelde 1 tamamen şeffaftır)
            return renderer.DODissolve(1f, duration).SetEase(Ease.InOutQuad);
        }

        /// <summary>
        /// Nesneyi görünür yapar (Dissolve In)
        /// </summary>
        public static Tween DODissolveIn(this Renderer renderer, float duration = 1f)
        {
            // 1'den 0'a iner
            return renderer.DODissolve(0f, duration).SetEase(Ease.InOutQuad);
        }

        /// <summary>
        /// Orijinal kodundaki gibi sürekli yanıp sönme efekti (Pulse)
        /// </summary>
        public static Tween DODissolvePulse(this Renderer renderer, float duration = 1f)
        {
            // 0'dan 1'e git, sonra geri gel (Yoyo), ve bunu sonsuza kadar yap (Loop: -1)
            return renderer.DODissolve(1f, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}