# -=-=-=-=-    COLOURS -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

DEF_COLOR   = \033[0;39m
YELLOW      = \033[0;93m
CYAN        = \033[0;96m
GREEN       = \033[0;92m
BLUE        = \033[0;94m
RED         = \033[0;91m

# -=-=-=-=-    NAME -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= #

NAME        := computor
PROJECT     := computorv1

# -=-=-=-=-    DOTNET SETTINGS -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

DOTNET      = dotnet

# -=-=-=-=-    PATH -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

RM          = rm -fr
BUILD_DIR   = bin
OBJ_DIR     = obj

# -=-=-=-=-    FILES -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

CSPROJ      = $(PROJECT).csproj
SOURCES     = Core/CustomMath.cs \
				Core/Polynomial.cs \
				Core/PolynomialParser.cs \
				Core/PolynomialSolver.cs \
				Core/SolutionResult.cs \
				Core/Term.cs \
				IO/InputHandler.cs \
				IO/OutputHandler.cs
EXECUTABLE  = $(BUILD_DIR)/Release/net8.0/$(PROJECT)

# -=-=-=-=-    TARGETS -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- #

all: $(NAME)

$(NAME): $(SOURCES) $(CSPROJ)
	@echo "$(YELLOW)Building $(PROJECT)...$(DEF_COLOR)"
	@$(DOTNET) build $(BUILD_FLAGS)
	@echo "$(RED)Poly-facetic, Poly-carpian, Poly-nomial$(DEF_COLOR)"

build : $(NAME)

run: $(NAME)
	@echo "$(CYAN)Running $(NAME)...$(DEF_COLOR)"
	@$(DOTNET) run $(RUN_FLAGS)

clean:
	@echo "$(YELLOW)Cleaning build artifacts...$(DEF_COLOR)"
	@$(DOTNET) clean --verbosity quiet 2>/dev/null || true
	@$(RM) $(BUILD_DIR) $(OBJ_DIR)
	@echo "$(RED)Cleaned object files and build artifacts$(DEF_COLOR)"

fclean: clean
	@$(RM) $(NAME) $(PUBLISH_DIR)
	@echo "$(RED)Cleaned all binaries$(DEF_COLOR)"

re: fclean all

info:
	@echo "$(BLUE)Project Information:$(DEF_COLOR)"
	@echo "  Name: $(NAME)"
	@echo "  Project: $(PROJECT)"
	@echo "  .NET Version: $$($(DOTNET) --version 2>/dev/null || echo 'Not installed')"
	@echo "  Sources: $$(echo '$(SOURCES)' | wc -w) files"

.PHONY: all build run test publish restore clean fclean re format check info help