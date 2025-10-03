using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HanClothesShopProject.Models
{
    public partial class dbContext : DbContext
    {
        public dbContext()
        {
        }

        public dbContext(DbContextOptions<dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<ApplyReturn> ApplyReturns { get; set; } = null!;
        public virtual DbSet<Article> Articles { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public virtual DbSet<Coupon> Coupons { get; set; } = null!;
        public virtual DbSet<GetCoupon> GetCoupons { get; set; } = null!;
        public virtual DbSet<ImageChart> ImageCharts { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderComment> OrderComments { get; set; } = null!;
        public virtual DbSet<OrdersDetail> OrdersDetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductSave> ProductSaves { get; set; } = null!;
        public virtual DbSet<ReturnSet> ReturnSets { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("address");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Area)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("area");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Detail)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("detail");

                entity.Property(e => e.Mark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mark");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("phone")
                    .IsFixedLength();

                entity.Property(e => e.Province)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("province");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_address_user");
            });

            modelBuilder.Entity<ApplyReturn>(entity =>
            {
                entity.ToTable("apply_return");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BusinessMark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("business_mark");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.ReturnReason)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("return_reason");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.UserMark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_mark");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ApplyReturns)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_apply_return_orders");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ApplyReturns)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_apply_return_product");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.ApplyReturns)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_apply_return_user");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("article");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Detail)
                    .HasColumnType("text")
                    .HasColumnName("detail");

                entity.Property(e => e.Sight).HasColumnName("sight");

                entity.Property(e => e.Title)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_cart_product");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_cart_user");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Catename)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("catename");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("chat_message");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FromUserid).HasColumnName("from_userid");

                entity.Property(e => e.IsRead).HasColumnName("is_read");

                entity.Property(e => e.Message)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("message");

                entity.Property(e => e.SendTime)
                    .HasColumnType("datetime")
                    .HasColumnName("send_time");

                entity.Property(e => e.ToUserid).HasColumnName("to_userid");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.ToTable("coupons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("discount_amount");

                entity.Property(e => e.DiscountType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("discount_type");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("end_date");

                entity.Property(e => e.ShouldMoney)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("should_money");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.SumCount).HasColumnName("sum_count");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.Property(e => e.UsedCount).HasColumnName("used_count");
            });

            modelBuilder.Entity<GetCoupon>(entity =>
            {
                entity.ToTable("get_coupon");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.IsUse)
                    .HasColumnName("is_use")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Coupon)
                    .WithMany(p => p.GetCoupons)
                    .HasForeignKey(d => d.CouponId)
                    .HasConstraintName("FK_get_coupon_coupons");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GetCoupons)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_get_coupon_user");
            });

            modelBuilder.Entity<ImageChart>(entity =>
            {
                entity.ToTable("image_chart");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("image_url");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Url)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("url");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddressId).HasColumnName("address_id");

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.DecMoney)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("dec_money");

                entity.Property(e => e.ExpressName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("express_name");

                entity.Property(e => e.ExpressNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("express_number");

                entity.Property(e => e.IsPay).HasColumnName("is_pay");

                entity.Property(e => e.Mark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mark");

                entity.Property(e => e.OrderNum)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("order_num");

                entity.Property(e => e.PayWay)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("pay_way");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.SumPrice)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("sum_price");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK_orders_user");
            });

            modelBuilder.Entity<OrderComment>(entity =>
            {
                entity.ToTable("order_comment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Detail)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("detail");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderComments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_order_comment_orders");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.OrderComments)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_order_comment_product");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OrderComments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_order_comment_user");
            });

            modelBuilder.Entity<OrdersDetail>(entity =>
            {
                entity.ToTable("orders_detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Aid).HasColumnName("aid");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.SumPrice)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("sum_price");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrdersDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_orders_detail_orders");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.OrdersDetails)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_orders_detail_product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cid).HasColumnName("cid");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Detail)
                    .HasColumnType("text")
                    .HasColumnName("detail");

                entity.Property(e => e.Img)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("img");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.Postage)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("postage");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.SalePrice)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("sale_price");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasDefaultValueSql("((10))");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.CidNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.Cid)
                    .HasConstraintName("FK_product_category");
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.ToTable("product_attribute");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Title)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("title");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_attribute_product");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("product_images");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("image_url");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_product_images_product");
            });

            modelBuilder.Entity<ProductSave>(entity =>
            {
                entity.ToTable("product_save");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Pid).HasColumnName("pid");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.ProductSaves)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_product_save_product");

                entity.HasOne(d => d.UidNavigation)
                    .WithMany(p => p.ProductSaves)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_product_save_user");
            });

            modelBuilder.Entity<ReturnSet>(entity =>
            {
                entity.ToTable("return_set");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Area)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("area");

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("city");

                entity.Property(e => e.Createtime)
                    .HasColumnType("datetime")
                    .HasColumnName("createtime");

                entity.Property(e => e.Detail)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("detail");

                entity.Property(e => e.Mark)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("mark");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("phone")
                    .IsFixedLength();

                entity.Property(e => e.Province)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("province");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Img)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("img");

                entity.Property(e => e.Introduce)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("introduce");

                entity.Property(e => e.Mibao)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("mibao");

                entity.Property(e => e.Nickname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("nickname");

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Pwd)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("pwd");

                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Sex)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("sex")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
