using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventsWebApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Location = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventUsers",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    EventId = table.Column<Guid>(type: "char(36)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUsers", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventUsers_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokenEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Token = table.Column<string>(type: "longtext", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokenEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokenEntity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Category", "DateTime", "Description", "ImageUrl", "Location", "MaxUsers", "Name" },
                values: new object[,]
                {
                    { new Guid("084bf991-dc7d-4420-a241-9f90a3491672"), "Концерты", new DateTime(2024, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Каждый вторник у тебя есть возможность стать участником комедийно-импровизационного шоу с астрологом!\n\nС тебя: дата, время, место рождения, волнующий вопрос (отношения, деньги, карьера и др.) и смелость выйти на сцену.\n\nС нас: шутки и советы от астролога.\n\nТебе точно понравится, ждём!\n\nДля Вашего комфорта просим приходить минимум за 20 минут до начала шоу (начало в 20-00), это позволит Вам комфортно расположиться за столиком, а так, же сделать заказ и пообщаться с нашими барменами.\n\nОбращаем Ваше внимание, что Вы бронируете места за столиком либо за барной стойкой.\n\nМеста за столиком рассчитаны на 4-х человек, поэтому просим Вас дружелюбно отнестись к возможной подсадке и гостям рядом с Вами. Надеемся на Ваше понимание и позитивное настроение.\n\nБронирование мест, ежедневно с 11-00 до 19-00.", "/images/8e4a505ad6ed6fe793a128e143ee85f5.jpg", "Брест", 500, "Астро разборы" },
                    { new Guid("436427a2-2430-4be1-bc77-ad48dcb3f9b3"), "Вечеринки", new DateTime(2024, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dasha Sova. Минск.", "/images/8c00e3aec2d63516b28d797d5425c05d.jpg", "Минск", 900, "Dasha Sova" },
                    { new Guid("43677c87-abf5-4b95-aaba-c9f66ed166fc"), "Форумы", new DateTime(2024, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "26 сентября в Минске пройдет Belarus Retail & Real Estate Forum 2024 с участием экспертов рынка коммерческой и жилой недвижимости, ритейла и маркетинга. Спикеры мероприятия расскажут об актуальных тенденциях и эффективных инструментах развития бизнеса в рамках единственной в стране отраслевой Премии Belarus Retail & Real Estate Awards 2024.\n\nФорум соберет представителей управляющих компаний, инвесторов, технологических компаний, консультантов, девелоперов, ритейлеров, специалистов в сферах торговой и офисной недвижимости, владельцов бизнес-центров и других специалистов из сферы торговли.\n\nКлючевыми темами для обсуждения на Belarus Retail & Real Estate Forum 2024 станут:\n\n– Потребительские тренды (результаты исследования компании МАСМИ);\n\n– Аналитика, динамика и перспективы развития рынка коммерческой недвижимости Беларуси;\n\n– Как включить в корпоративную культуру представителей поколения Z и снизить текучку;\n\n– Как правильно открывать торговые точки в разных локациях;\n\n– Систематизация и оцифровка бизнеса в ритейле;\n\n– Коллаборации локальных fashion-брендов с крупными компаниями;\n\n– Как вести эффективные переговоры в коммерческой недвижимости и ритейле, чтобы достигать своих целей;\n\n– Диджитализация деятельности застройщика и девелопера;\n\n– Как увеличить продажи в 3 раза с помощью визуальной концепции и мерчандайзинга и другие.\n\nВ качестве спикеров на форум приглашены больше десятка экспертов: руководитель проектов агентства социальных и маркетинговых исследований MASMI Belarus Анастасия Печко, генеральный директор ООО «Звук Бизнес» Сергей Майоров, СEO и сооснователь сети «Зеленая аптека Беларуси» и медицинских центров «Эксана» Ругия Кенгерли, партнер и исполнительный директор NAI Belarus Андрей Алёшкин, основательница фэшн-бренда UNO MAS UNO Татьяна Лавренова, основатель международной компании VMC Retail и специалист по фэшн-мерчендайзингу и проектированию магазинов Марина Полковникова, бизнес-консультант и основатель школы управления «Менеджетик» Александр Самойлов, аналитик консалтинговой компании «Коллиерз» Дмитрий Соловых, профориентатор и игропедагог Ирина Тузина, системный предприниматель и собственник брендов женской одежды EL VIENTO, PERRA, KISSONS Василий Ануфриев, начальник службы маркетинговых коммуникаций сети магазинов «5 Элемент» Наталья Хатьянова, учредитель OOO «RealtyProTechnology» Александр Николайчук и другие.", "/images/cef1849684e6681f1f17b97e0112d274.jpg", "Минск", 240, "Belarus Retail & Real Estate Forum 2024" },
                    { new Guid("647a4dfe-18e2-4d7b-ba86-3eb1163d914d"), "Выставки", new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "На выставке «Самые загадочные картины мира» вы увидите более 80 невероятных сюжетов мировых художников — Сандро Боттичелли, Леонардо Да Винчи, Тициан Вечеллио, Иероним Босх, Питер Брейгель, Ханс Бальдунг, Караваджо, Франсиско Гойя, Фрида Кало, Билл Стоунхэй, Гюстав Моро, Винсент Ван Гог, Эдвард Мунк, Иван Крамской, которые обладают такой мощной силой, что буквально гипнотизируют зрителей. Их можно рассматривать часами, пытаясь разгадать смыслы, которые стремился донести художник.", "/images/844490352bc7fd1d1a69740b699b5de7.jpg", "Витебск", 30, "Выставка «Самые загадочные картины мира»" },
                    { new Guid("865ff223-6de7-4a92-a3ed-81e5759f8b23"), "Форумы", new DateTime(2024, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Crypto Bridge —  крупнейший в Беларуси международный форум по Blockchain, Banking & Finance, DeFi, Security, Dev, Ai & Metaverse.\n\nС 10:00 - 20:00 (регистрация начинается с 8:00)\n\nУчастники\n\nБолее 2000 участников из 20 стран (РБ, РФ, Казахстан, Узбекистан, Кыргызстан), 2 дня погружения в тему, более 80 ТОП-спикеров, стендовые зоны, открытые и закрытые сессии. \n\nКрупнейшие игроки индустрии, должностные лица, частные инвесторы, фонды, банки, представители инновационных стартапов и криптовалют, разработчики блокчейн-технологий, ключевые участники майнинг-сектора и многие другие.\nОнлайн-трансляция в Метавселенной: аудитория более 200 000 человек\n\nНетворкинг\n\nФорум предоставляет отличную площадку для установления бизнес-контактов, обмена идеями, партнерств и потенциальных инвестиций.\n\nТемы форума\n\n- Блокчейн технологии \n - Обработка данных\n - Финтех и банкинг \n - Искусственный интеллект \n - Информационная безопасность \n - Регулирование и правовые аспекты (в Республике Беларусь и за рубежом);\n - Аудит компаний и проектов \n - Развитие индустрии продуктовых стартапов\n\nДокладчики — хэдлайнеры отрасли, такие как: Владимир Смеркис (Директор криптобиржи Binance в России и СНГ), Василий Кулеш (Председатель блокчейн-ассоциации Беларуси)\n\nОрганизатор\n\nМероприятие организовано медийным порталом futureby.info при поддержке Блокчейн Ассоциации Республики Беларусь и Российской ассоциации криптовалют и блокчейна (РАКИБ).\nСледите за актуальной информацией в telegram-канале @future_by и канале Форума @cypto_bridge.", "/images/5fc64073e6833b2f98899993ce1ce18c.jpg", "Витебск", 200, "Crypto Bridge" },
                    { new Guid("a9def34d-f29c-4834-823c-c947cedac333"), "Концерты", new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "27 september Astro.Club.Minsk Приглашает вас на официальное открытие.\nSpecial guest Кравц @kravzzz\n\nПогрузитесь в мир космической атмосферы клуба, а так же вечеринки, которой всем нам так не хватает.\nЯркие люди, брызги шампанского.\n\nGrand Opening Party\nSmart Casual\n\nБудем рады внести вас в список наших дорогих гостей.", "/images/8e47918e1675cd40dbb14efbb71ff3c7.jpg", "Брест", 500, "Grand Opening Party" },
                    { new Guid("db37e9be-af89-4caf-ba43-2fc0a1f3f547"), "Концерты", new DateTime(2024, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Впервые в Минске! Грандиозная премьера от легендарного Imperial Orchestra, Яндекс Афиши и Плюс Студии — Симфоническая панк-сказка «Король и Шут»!\n\nЕдинственное официальное шоу, которое уже увидело 70000 восторженных зрителей!\n\nПосещение трибун для детей с 6 до 16 лет — только в сопровождении взрослых. Посещение танцпола возможно от 16 лет.\n\nЛюбимые песни культовой группы превратятся в большое путешествие по сказочным мирам в сопровождении симфонического оркестра, хора и органа.\n\nНа многоуровневой сцене разместятся более ста виртуозных музыкантов Imperial Orchestra в костюмах персонажей из песен «Короля и Шута», а дирижировать сказочным оркестром будет сам Мёртвый Анархист. Симфоническую сказку оживляет атмосферный видеоряд а ведущим в фэнтези-мир станет трехмерная голова Шута из одноименного сериала!\n\nСвыше 250 квадратных метров Led-экранов, масштабное световое шоу, мощный живой звук, неповторимая атмосфера легендарных концертов группы и самый большой в истории хор - 15000 человек, а именно Вас, дорогие зрители!\n\nТанцуйте и подпевайте любимым песням на танцполе или слушайте и отдыхайте в ложе. Слияние панк-энергии и симфонических инструментов, мистическая атмосфера и новая мрачная история по мотивам творчества группы — в симфонической панк-сказке есть всё, как для старых фанатов группы, так и для тех, кто присоединился совсем недавно.\n\nЖдём всех, кто знает, почему не стоит оставаться в гостях у лесника, ужинать с конюхом и ходить в чужой сад по цветы — или просто очень любит панк.\n\nТрек-лист: «Камнем по голове», «Смельчак и ветер», «Мёртвый анархист», «Лесник», «Дурак и молния», «Охотник», «Проклятый старый дом», «Два вора и монета», «Внезапная голова», «Девушка и Граф», «Ели мясо мужики», «Ведьма и осёл», «Прыгну со скалы», «Помнят с горечью древляне», «Проказник скоморох», «Ром», «Джокер», «Исповедь вампира», «Воспоминания о былой любви», «Северный флот», «Мастер приглашает в гости», «Фокусник», «Танец злобного гения», «Медведь», «Марионетки», «Кукла колдуна», «Гимн шута», «Утренний рассвет».", "/images/ec0c3b21b69e4ee1446cc4bb8385948f.jpg", "Минск", 1000, "Симфоническая панк-сказка «Король и Шут»" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Email", "FirstName", "LastName", "PasswordHash", "Role" },
                values: new object[] { new Guid("632d7af4-8c37-4235-ae45-b0dbf7451014"), null, "admin@gmail.com", "", "", "$2a$11$Je2fG7m05UbM0eEFR1W4oeV2i/w/mEdfM/FpfcDDXutgzGkdVnr7.", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokenEntity_UserId",
                table: "RefreshTokenEntity",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventUsers");

            migrationBuilder.DropTable(
                name: "RefreshTokenEntity");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
