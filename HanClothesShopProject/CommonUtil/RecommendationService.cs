using HanClothesShopProject.Models;
using Microsoft.EntityFrameworkCore;

namespace HanClothesShopProject.CommonUtil
{
    //協同算法 推薦商品共用類
    public class RecommendationService
    {
        private readonly dbContext _context;

        // 透過「依賴注入」取得資料庫 DbContext
        public RecommendationService(dbContext context)
        {
            _context = context;
        }

        // 取得推薦商品清單（已登入使用者）
        public List<Product> GetRecommendedProductsAsync(int userId, int topN = 10)
        {
            // 取得目前使用者的評分資料（包含使用者曾評分過的商品）
            // ⚠️ 這裡之後可依效能需求再做調整
            var userRatings = _context.OrderComments
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.PidNavigation)
                .ToList();

            // 取得所有商品以及其對應的評分資料
            // ⚠️ 資料量大時建議優化（避免一次撈全部）
            var allProducts = _context.Products
                .Include(p => p.OrderComments)
                .ToList();

            // 用來儲存每個商品的「推薦分數」
            var productScores = new Dictionary<int, double>();

            // 針對使用者已評分的商品，尋找相似的其他商品
            foreach (var rating in userRatings)
            {
                // 計算與目前商品相似的其他商品，並依相似度由高到低排序
                var similarProducts = allProducts
                    .Where(p => p.Id != rating.PidNavigation.Id) // 排除自己本身
                    .Select(p => new
                    {
                        Product = p,
                        Similarity = CalculateSimilarity(
                            rating.PidNavigation,
                            p
                        ) // 計算商品之間的相似度
                    })
                    .OrderByDescending(p => p.Similarity);

                // 累加每個相似商品的推薦分數
                foreach (var similarProduct in similarProducts)
                {
                    // 如果該商品已存在分數，則進行加總
                    if (productScores.ContainsKey(similarProduct.Product.Id))
                    {
                        productScores[similarProduct.Product.Id] +=
                            similarProduct.Similarity * (rating.Score ?? 0);
                    }
                    else
                    {
                        // 尚未存在則初始化分數
                        productScores[similarProduct.Product.Id] =
                            similarProduct.Similarity * (rating.Score ?? 0);
                    }
                }
            }

            // 依推薦分數由高到低，回傳前 N 筆商品
            var list= productScores
                .OrderByDescending(p => p.Value)
                .Take(topN)
                .Select(p => allProducts.First(prod => prod.Id == p.Key))
                .ToList();
            if (list.Count == 0)
            {
                list= this.GetPopularProductsAsync(topN);
            }
            return list;
        }

        // 計算兩個商品之間的相似度
        private double CalculateSimilarity(Product product1, Product product2)
        {
            // 找出「同時評分過這兩個商品」的使用者
            var commonUsers = product1.OrderComments
                .Select(r => r.UserId)
                .Intersect(
                    product2.OrderComments.Select(r => r.UserId)
                )
                .ToList();

            // 若沒有共同評分的使用者，相似度直接回傳 0
            if (!commonUsers.Any())
                return 0;

            // 使用「皮爾森相關係數（Pearson Correlation）」計算相似度
            double sum1 = 0, sum2 = 0;
            double sum1Sq = 0, sum2Sq = 0;
            double pSum = 0;
            int n = commonUsers.Count;

            // 逐一處理共同使用者的評分
            foreach (var userId in commonUsers)
            {
                // 取得該使用者對兩個商品的評分
                var rating1 = product1.OrderComments
                    .First(r => r.UserId == userId)
                    .Score ?? 0;

                var rating2 = product2.OrderComments
                    .First(r => r.UserId == userId)
                    .Score ?? 0;

                // 累加評分資料
                sum1 += rating1;
                sum2 += rating2;

                sum1Sq += Math.Pow(rating1, 2);
                sum2Sq += Math.Pow(rating2, 2);

                pSum += rating1 * rating2;
            }

            // 計算皮爾森相關係數公式
            double num = pSum - (sum1 * sum2 / n);
            double den = Math.Sqrt(
                (sum1Sq - Math.Pow(sum1, 2) / n) *
                (sum2Sq - Math.Pow(sum2, 2) / n)
            );

            // 若分母為 0，代表無法計算相似度
            if (den == 0)
                return 0;

            // 回傳相似度結果（介於 -1 ~ 1）
            return num / den;
        }

        // 使用者未登入時，回傳熱門商品清單
        public List<Product> GetPopularProductsAsync(int topN = 10)
        {
            // 依商品總評分由高到低排序
            var popularProducts = _context.Products
                .OrderByDescending(p => p.Score)
                .Take(topN)
                .ToList();

            return popularProducts;
        }
    }
}
